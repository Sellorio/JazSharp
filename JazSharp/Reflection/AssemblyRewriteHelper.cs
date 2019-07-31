using JazSharp.SpyLogic;
using JazSharp.Testing;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.Linq;

namespace JazSharp.Reflection
{
    internal static class AssemblyRewriteHelper
    {
        private static readonly string[] DoNotWrapAssemblies =
        {
            typeof(Jaz).Assembly.FullName
        };

        private static readonly string[] DoNotRecurseAssemblies =
        {
            typeof(Jaz).Assembly.FullName,
            typeof(string).Assembly.FullName
        };

        internal static void RewriteAssembly(string filename, TestCollection forTests)
        {
            var anyFocused = forTests.Tests.Any(x => x.IsFocused);
            var assembly = AssemblyDefinition.ReadAssembly(filename, new ReaderParameters { ReadWrite = true });

            var testMethodDefinitions =
                forTests.Tests
                    .Where(x => (!anyFocused || x.IsFocused) && !x.IsExcluded && x.AssemblyName == assembly.FullName)
                    .Select(x =>
                        assembly.MainModule
                            .Types.First(y => y.MetadataToken.ToInt32() == x.TestClassMetadataToken)
                            .Methods.First(y => y.MetadataToken.ToInt32() == x.TestMetadataToken))
                    .ToList();
            
            var processedMethods = new List<MethodDefinition>();

            foreach (var testMethodDefinition in testMethodDefinitions)
            {
                RewriteAssembly(testMethodDefinition, processedMethods);
            }

            assembly.Write(filename);
        }

        private static void RewriteAssembly(MethodDefinition method, List<MethodDefinition> processedMethods)
        {
            var dummyVariable = method.ReturnType.FullName != "System.Void" ? new VariableDefinition(method.ReturnType) : null;
            method.Body.Variables.Add(dummyVariable);
            var ilProcessor = method.Body.GetILProcessor();
            var replacedMethods = new List<MethodDefinition>();

            foreach (var instruction in method.Body.Instructions)
            {
                if (instruction.Operand is MethodDefinition calledMethod)
                {
                    if (!DoNotWrapAssemblies.Contains(calledMethod.DeclaringType.Module.Assembly.FullName))
                    {
                        var parameterCount = calledMethod.IsStatic ? calledMethod.Parameters.Count : calledMethod.Parameters.Count + 1;
                        var entryMethodName = (calledMethod.ReturnType.FullName == "System.Void" ? "Action" : "Func") + parameterCount;
                        var genericMethod = new GenericInstanceMethod(method.Module.ImportReference(typeof(SpyEntryPoints).GetMethod(entryMethodName)));

                        if (!calledMethod.IsStatic)
                        {
                            genericMethod.GenericArguments.Add(calledMethod.DeclaringType);
                        }

                        foreach (var parameter in calledMethod.Parameters)
                        {
                            genericMethod.GenericArguments.Add(parameter.ParameterType);
                        }

                        replacedMethods.Add(calledMethod);
                        ilProcessor.Replace(instruction, Instruction.Create(OpCodes.Call, genericMethod));
                    }
                }
            }

            foreach (var replacedMethod in replacedMethods)
            {
                ilProcessor.Append(Instruction.Create(OpCodes.Ldc_I4, replacedMethod.MetadataToken.ToInt32()));
            }

            if (dummyVariable != null)
            {
                ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, dummyVariable.Index));
            }

            ilProcessor.Append(Instruction.Create(OpCodes.Ret));

            processedMethods.Add(method);

            foreach (var replacedMethod in replacedMethods.Except(processedMethods))
            {
                if (!DoNotRecurseAssemblies.Contains(replacedMethod.DeclaringType.Module.Assembly.FullName))
                {
                    RewriteAssembly(replacedMethod, processedMethods);
                }
            }
        }
    }
}
