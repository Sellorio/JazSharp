using JazSharp.SpyLogic;
using JazSharp.Testing;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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
                    var genericMethod = new GenericInstanceMethod(module.ImportReference(entryMethod));

                    if (isFunc)
                    {
                        genericMethod.GenericArguments.Add(calledMethod.ReturnType);
                    }

                    if (calledMethod.HasThis)
                    {
                        genericMethod.GenericArguments.Add(calledMethod.DeclaringType);
                    }

                    foreach (var parameter in calledMethod.Parameters)
                    {
                        genericMethod.GenericArguments.Add(parameter.ParameterType);
                    }

                    replacedMethods.Add(calledMethod);

                    return Instruction.Create(OpCodes.Call, genericMethod);
                }
            }

            return null;
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
                SerializeGenericParameters(serializedInfo, genericType.GenericParameters);
            }

            serializedInfo
                .Append(':')
                .Append(MethodName(resolvedReplacedMethod));

            if (method is GenericInstanceMethod genericMethod)
            {
                SerializeGenericParameters(serializedInfo, genericMethod.GenericParameters);
            }

            return serializedInfo.ToString();
        }

        private static void SerializeGenericParameters(StringBuilder serializedInfo, Collection<GenericParameter> genericParameters)
        {
            serializedInfo.Append('?');
            var first = true;

            foreach (var arg in genericParameters)
            {
                if (!first)
                {
                    serializedInfo.Append(' ');
                }

                serializedInfo.Append(TypeName(arg));
                first = false;
            }
        }

        private static string TypeName(TypeReference type)
        {
            type = type.Resolve() ?? type;
            return BuiltInValueTypes.Contains(type.ToString()) ? type.Name : type.ToString().Replace('/', '+');
        }

        private static string MethodName(MethodDefinition method)
        {
            return $"{TypeName(method.ReturnType)} {method.Name}({string.Join(", ", method.Parameters.Select(x => TypeName(x.ParameterType)))})";
        }
    }
}
