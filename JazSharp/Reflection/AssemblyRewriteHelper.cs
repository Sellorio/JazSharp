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

        internal static void RewriteAssembly(string filename, TestCollection forTests)
        {
            var anyFocused = forTests.Tests.Any(x => x.IsFocused);

            using (var assembly = AssemblyDefinition.ReadAssembly(filename, new ReaderParameters { ReadWrite = true, ReadSymbols = true }))
            {
                foreach (var test in forTests.Tests.Where(x => (!anyFocused || x.IsFocused) && !x.IsExcluded && x.AssemblyName == assembly.FullName))
                {
                    var testMethod = (MethodDefinition)assembly.MainModule.LookupToken(test.TestMetadataToken);
                    RewriteAssembly(testMethod, new List<MethodDefinition>());
                }

                assembly.Write(new WriterParameters { WriteSymbols = true });
            }
        }

        private static void RewriteAssembly(MethodDefinition method, List<MethodDefinition> processedMethods)
        {
            VariableDefinition dummyVariable = null;

            if (method.ReturnType.FullName != "System.Void")
            {
                dummyVariable = new VariableDefinition(method.ReturnType);
                method.Body.Variables.Add(dummyVariable);
            }

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
                        ilProcessor.Replace(instruction, replacement);
                    }
                }
                else if (instruction.OpCode == OpCodes.Ldftn)
                {
                    functionsAsParameters.Add((MethodReference)instruction.Operand);
                }
            }

            foreach (var replacedMethod in replacedMethods)
            {
                // Needed to get correct module/assembly for declaring type as well as the correct metadata token.
                var resolvedReplacedMethod = replacedMethod.Resolve();
                var serializedInfo = new StringBuilder();

                serializedInfo
                    .Append(resolvedReplacedMethod.DeclaringType.Module.Assembly.Name.Name)
                    .Append(':')
                    .Append(TypeName(resolvedReplacedMethod.DeclaringType));

                if (replacedMethod.DeclaringType is GenericInstanceType genericType)
                {
                    SerializeGenericArguments(serializedInfo, genericType.GenericArguments);
                }

                serializedInfo
                    .Append(':')
                    .Append(MethodName(resolvedReplacedMethod));

                if (replacedMethod is GenericInstanceMethod genericMethod)
                {
                    SerializeGenericArguments(serializedInfo, genericMethod.GenericArguments);
                }

                ilProcessor.Append(Instruction.Create(OpCodes.Ldstr, serializedInfo.ToString()));
            }

            if (dummyVariable != null)
            {
                ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, dummyVariable));
            }

            ilProcessor.Append(Instruction.Create(OpCodes.Ret));

            processedMethods.Add(method);

            var methodsToRewrite =
                Enumerable.Concat(replacedMethods, functionsAsParameters)
                    .Select(x => x as MethodDefinition ?? x.Resolve())
                    .Where(x => x != null && !DoNotRecurseAssemblies.Contains(x.DeclaringType.Module.Assembly.Name.Name))
                    .Except(processedMethods)
                    .ToList();

            foreach (var methodToRewrite in methodsToRewrite)
            {
                RewriteAssembly(methodToRewrite, processedMethods);
            }
        }

        private static Instruction GetSpyInstruction(ModuleDefinition module, MethodReference calledMethod, List<MethodReference> replacedMethods)
        {
            if (calledMethod != null)
            {
                if (!DoNotWrapAssemblies.Contains(calledMethod.DeclaringType.Resolve().Module.Assembly.Name.Name))
                {
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
                        var parameterType = parameter.ParameterType;

                        if (parameterType.IsGenericParameter)
                        {
                            parameterType = CecilHelper.ResolveGenericParameter(parameterType, calledMethod, calledMethod.DeclaringType);
                        }

                        genericMethod.GenericArguments.Add(parameterType);
                    }

                    replacedMethods.Add(calledMethod);

                    return Instruction.Create(OpCodes.Call, genericMethod);
                }
            }

            return null;
        }

        private static void SerializeGenericArguments(StringBuilder serializedInfo, Collection<TypeReference> genericArguments)
        {
            serializedInfo.Append('?');
            var first = true;

            foreach (var arg in genericArguments)
            {
                if (!first)
                {
                    serializedInfo.Append(' ');
                }

                var resolvedArg = arg.Resolve();
                serializedInfo
                    .Append(resolvedArg.Module.Assembly.Name.Name)
                    .Append('/')
                    .Append(TypeName(resolvedArg));

                first = false;
            }
        }

        private static string TypeName(TypeDefinition type)
        {
            return BuiltInValueTypes.Contains(type.ToString()) ? type.Name : type.ToString().Replace('/', '+');
        }

        private static string MethodName(MethodDefinition method)
        {
            return $"{TypeName(method.ReturnType.Resolve())} {method.Name}({string.Join(", ", method.Parameters.Select(x => TypeName(x.ParameterType.Resolve())))})";
        }
    }
}
