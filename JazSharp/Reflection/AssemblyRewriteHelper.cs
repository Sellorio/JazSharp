using JazSharp.SpyLogic;
using JazSharp.Testing;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace JazSharp.Reflection
{
    internal static class AssemblyRewriteHelper
    {
        private static readonly string[] DoNotWrapAssemblies =
        {
            typeof(Jaz).Assembly.GetName().Name
        };

        private static readonly string[] DoNotRecurseAssemblies =
        {
            "System.Private.CoreLib",
            "System.Console",
            typeof(Jaz).Assembly.GetName().Name
        };

        private static readonly string[] BuiltInValueTypes =
        {
            typeof(void).ToString(),
            typeof(bool).ToString(),
            typeof(byte).ToString(),
            typeof(short).ToString(),
            typeof(int).ToString(),
            typeof(long).ToString(),
            typeof(float).ToString(),
            typeof(double).ToString(),
            typeof(decimal).ToString()
        };

        internal static void RewriteAssembly(string filename)
        {
            using (var assembly = AssemblyDefinition.ReadAssembly(filename, new ReaderParameters { ReadWrite = true, ReadSymbols = true }))
            {
                foreach (var type in assembly.MainModule.Types.Where(x => !x.IsInterface))
                {
                    RewriteType(type);
                }

                assembly.Write(new WriterParameters { WriteSymbols = true });
            }
        }

        private static void RewriteType(TypeDefinition type)
        {
            foreach (var method in type.Methods.Where(x => !x.IsAbstract))
            {
                RewriteMethod(method);
            }

            foreach (var subType in type.NestedTypes.Where(x => !x.IsInterface))
            {
                RewriteType(subType);
            }
        }

        private static void RewriteMethod(MethodDefinition method)
        {
            var ilProcessor = method.Body.GetILProcessor();
            var replacedMethods = new List<MethodReference>();
            var functionsAsParameters = new List<MethodReference>();

            // needs to be for to stop exception when replacing instructions
            for (var i = 0; i < method.Body.Instructions.Count; i++)
            {
                var instruction = method.Body.Instructions[i];

                if (instruction.OpCode == OpCodes.Call || instruction.OpCode == OpCodes.Callvirt)
                {
                    var calledMethod = (MethodReference)instruction.Operand;
                    var replacement = GetSpyInstruction(method.Module, calledMethod, replacedMethods);

                    if (replacement != null)
                    {
                        ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldstr, SerializeMethodCall(calledMethod)));
                        ilProcessor.Replace(instruction, replacement);
                    }
                }
                else if (instruction.OpCode == OpCodes.Ldftn)
                {
                    functionsAsParameters.Add((MethodReference)instruction.Operand);
                }
            }
        }

        private static Instruction GetSpyInstruction(ModuleDefinition module, MethodReference calledMethod, List<MethodReference> replacedMethods)
        {
            if (calledMethod != null)
            {
                if (!DoNotWrapAssemblies.Contains(calledMethod.DeclaringType.Resolve().Module.Assembly.Name.Name))
                {
                    var resolvedMethod = calledMethod.Resolve();

                    if (resolvedMethod != null && resolvedMethod.IsConstructor)
                    {
                        return null;
                    }

                    var isFunc = calledMethod.ReturnType.FullName != "System.Void";
                    var parameterCount = !calledMethod.HasThis ? calledMethod.Parameters.Count : calledMethod.Parameters.Count + 1;
                    var entryMethodName = (isFunc ? "Func" : "Action") + parameterCount;
                    var entryMethod = typeof(SpyEntryPoints).GetMethod(entryMethodName, BindingFlags.Static | BindingFlags.Public);
                    var spyMethod = module.ImportReference(entryMethod);
                    var genericMethod = new GenericInstanceMethod(spyMethod);

                    if (isFunc)
                    {
                        genericMethod.GenericArguments.Add(ResolveTypeIfGeneric(calledMethod, calledMethod.ReturnType));
                    }

                    if (calledMethod.HasThis)
                    {
                        genericMethod.GenericArguments.Add(ResolveTypeIfGeneric(calledMethod, calledMethod.DeclaringType));
                    }

                    foreach (var parameter in calledMethod.Parameters)
                    {
                        genericMethod.GenericArguments.Add(ResolveTypeIfGeneric(calledMethod, parameter.ParameterType));
                    }

                    replacedMethods.Add(calledMethod);

                    return Instruction.Create(OpCodes.Call, genericMethod);
                }
            }

            return null;
        }

        private static TypeReference ResolveTypeIfGeneric(MethodReference method, TypeReference type)
        {
            if (type.IsGenericParameter)
            {
                int genericParameterIndex = -1;
                IGenericInstance genericArgSource = null;

                var indexBasedMatch = Regex.Match(type.Name, @"^\!(\!|)([0-9]+)$");

                if (indexBasedMatch.Success)
                {
                    genericParameterIndex = int.Parse(indexBasedMatch.Groups[2].Value);

                    if (indexBasedMatch.Groups[1].Value != string.Empty)
                    {
                        genericArgSource = (GenericInstanceMethod)method;
                    }
                    else
                    {
                        genericArgSource = (GenericInstanceType)method.DeclaringType;
                    }
                }
                else
                {
                    if (method is GenericInstanceMethod genericMethod)
                    {
                        var definition = genericMethod.Resolve();
                        var indexOfGenericParameter = IndexOfItem(definition.GenericParameters, x => x.Name == type.Name);

                        if (indexOfGenericParameter != -1)
                        {
                            genericParameterIndex = indexOfGenericParameter;
                            genericArgSource = genericMethod;
                        }
                    }

                    if (genericArgSource == null && method.DeclaringType is GenericInstanceType genericType)
                    {
                        var definition = genericType.Resolve();
                        var indexOfGenericParameter = IndexOfItem(definition.GenericParameters, x => x.Name == type.Name);

                        if (indexOfGenericParameter != -1)
                        {
                            genericParameterIndex = indexOfGenericParameter;
                            genericArgSource = genericType;
                        }
                    }
                }

                if (genericParameterIndex != -1 && genericArgSource != null)
                {
                    return genericArgSource.GenericArguments[genericParameterIndex];
                }
            }
            else if (type is GenericInstanceType genericType)
            {
                TypeReference genericTypeDefinition = genericType.Resolve();
                genericTypeDefinition = method.Module.ImportReference(genericTypeDefinition);
                var result = new GenericInstanceType(genericTypeDefinition);

                foreach (var arg in ((GenericInstanceType)type).GenericArguments)
                {
                    result.GenericArguments.Add(ResolveTypeIfGeneric(method, arg));
                }

                return result;
            }

            return type;
        }

        private static string SerializeMethodCall(MethodReference method)
        {
            var resolvedReplacedMethod = method.Resolve();
            var serializedInfo = new StringBuilder();

            serializedInfo
                .Append(resolvedReplacedMethod.DeclaringType.Module.Assembly.Name.Name)
                .Append(':')
                .Append(TypeName(resolvedReplacedMethod.DeclaringType));

            if (method.DeclaringType is GenericInstanceType genericType)
            {
                SerializeGenericArguments(serializedInfo, genericType.GenericArguments);
            }

            serializedInfo
                .Append(':')
                .Append(MethodDescription(resolvedReplacedMethod));

            if (method is GenericInstanceMethod genericMethod)
            {
                SerializeGenericArguments(serializedInfo, genericMethod.GenericArguments);
            }

            return serializedInfo.ToString();
        }

        private static void SerializeGenericArguments(StringBuilder serializedInfo, Collection<TypeReference> genericParameters)
        {
            serializedInfo.Append('?');
            var first = true;

            foreach (var arg in genericParameters)
            {
                if (!first)
                {
                    serializedInfo.Append(' ');
                }

                var resolvedArg = arg.IsGenericParameter ? arg : arg.Resolve();

                serializedInfo.Append(
                    resolvedArg.IsGenericParameter
                        ? resolvedArg.Name
                        : resolvedArg.Module.Assembly.Name.Name + "/" + resolvedArg.ToString().Replace('/', '+'));

                first = false;
            }
        }

        private static string TypeName(TypeReference type)
        {
            type = type.Resolve() ?? type;
            return BuiltInValueTypes.Contains(type.ToString()) ? type.Name : type.ToString().Replace('/', '+');
        }

        private static string MethodDescription(MethodDefinition method)
        {
            var genericArgString =
                method.HasGenericParameters
                    ? "[" + string.Join(", ", method.GenericParameters.Select(x => x.Name)) + "]"
                    : string.Empty;

            return $"{TypeName(method.ReturnType)} {method.Name}{genericArgString}({string.Join(", ", method.Parameters.Select(ParameterDescription))})";
        }

        private static string ParameterDescription(ParameterDefinition parameter)
        {
            if (BuiltInValueTypes.Contains(parameter.ParameterType.FullName))
            {
                return parameter.ParameterType.Name;
            }

            return parameter.ParameterType.ToString().Replace('/', '+').Replace('<', '[').Replace('>', ']');
        }

        private static int IndexOfItem<TObject>(IEnumerable<TObject> enumerable, Func<TObject, bool> matcher)
        {
            var index = 0;

            foreach (var item in enumerable)
            {
                if (matcher.Invoke(item))
                {
                    return index;
                }

                index++;
            }

            return -1;
        }
    }
}
