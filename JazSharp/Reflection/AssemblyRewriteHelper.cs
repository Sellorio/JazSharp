using JazSharp.SpyLogic;
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
        private static MethodInfo GetMethodFromHandleMethod =
            typeof(MethodBase)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(x => x.Name == "GetMethodFromHandle" && x.GetParameters().Length == 2);

        private static readonly string[] DoNotWrapAssemblies =
        {
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
            var getMethodByTokenMethod = method.Module.ImportReference(GetMethodFromHandleMethod);
            var ilProcessor = method.Body.GetILProcessor();

            // needs to be for loop to stop exception when replacing instructions
            for (var i = 0; i < method.Body.Instructions.Count; i++)
            {
                var instruction = method.Body.Instructions[i];

                if (instruction.OpCode == OpCodes.Call || instruction.OpCode == OpCodes.Callvirt)
                {
                    var calledMethod = (MethodReference)instruction.Operand;
                    var replacement = GetSpyInstruction(method.Module, calledMethod);

                    if (replacement != null)
                    {
                        ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldtoken, calledMethod));
                        ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldtoken, calledMethod.DeclaringType));
                        ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Call, getMethodByTokenMethod));

                        i += 3; // skip past newly inserted instructions

                        ilProcessor.Replace(instruction, replacement);
                    }
                }
            }
        }

        private static Instruction GetSpyInstruction(ModuleDefinition module, MethodReference calledMethod)
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
                        genericMethod.GenericArguments.Add(calledMethod.DeclaringType);
                    }

                    foreach (var parameter in calledMethod.Parameters)
                    {
                        genericMethod.GenericArguments.Add(ResolveTypeIfGeneric(calledMethod, parameter.ParameterType));
                    }

                    return Instruction.Create(OpCodes.Call, genericMethod);
                }
            }

            return null;
        }

        private static TypeReference ResolveTypeIfGeneric(MethodReference method, TypeReference type)
        {
            // reference to a type parameter on the method or it's declaring type
            if (type.IsGenericParameter)
            {
                int genericParameterIndex = -1;
                IGenericInstance genericArgSource = null;

                // references like !0 and !!0
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
                else // references like TValue
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

                if (genericParameterIndex == -1 || genericArgSource == null)
                {
                    throw new InvalidOperationException("Failed to resolve generic parameter when rewriting assembly.");
                }

                return genericArgSource.GenericArguments[genericParameterIndex];
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
