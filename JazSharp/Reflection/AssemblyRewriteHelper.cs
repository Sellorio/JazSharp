using JazSharp.SpyLogic;
using JazSharp.Testing;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            typeof(Jaz).Assembly.GetName().Name
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
            }

            foreach (var replacedMethod in replacedMethods)
            {
                ilProcessor.Append(Instruction.Create(OpCodes.Ldc_I4, replacedMethod.MetadataToken.ToInt32()));
            }

            if (dummyVariable != null)
            {
                ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, dummyVariable));
            }

            ilProcessor.Append(Instruction.Create(OpCodes.Ret));

            processedMethods.Add(method);

            foreach (var replacedMethod in replacedMethods.OfType<MethodDefinition>().Except(processedMethods))
            {
                if (!DoNotRecurseAssemblies.Contains(replacedMethod.DeclaringType.Module.Assembly.Name.Name))
                {
                    RewriteAssembly(replacedMethod, processedMethods);
                }
            }
        }

        private static Instruction GetSpyInstruction(ModuleDefinition module, MethodReference calledMethod, List<MethodReference> replacedMethods)
        {
            if (calledMethod != null)
            {
                if (!DoNotWrapAssemblies.Contains(calledMethod.DeclaringType.Module.Assembly.Name.Name))
                {
                    var parameterCount = !calledMethod.HasThis ? calledMethod.Parameters.Count : calledMethod.Parameters.Count + 1;
                    var entryMethodName = (calledMethod.ReturnType.FullName == "System.Void" ? "Action" : "Func") + parameterCount;
                    var entryMethod = typeof(SpyEntryPoints).GetMethod(entryMethodName, BindingFlags.Static | BindingFlags.Public);
                    var genericMethod = new GenericInstanceMethod(module.ImportReference(entryMethod));

                    if (calledMethod.HasThis)
                    {
                        genericMethod.GenericArguments.Add(calledMethod.DeclaringType);
                    }

                    foreach (var parameter in calledMethod.Parameters)
                    {
                        genericMethod.GenericArguments.Add(parameter.ParameterType);
                    }

                    if (!replacedMethods.Contains(calledMethod))
                    {
                        calledMethod = calledMethod.Resolve();
                        replacedMethods.Add(calledMethod);
                    }

                    return Instruction.Create(OpCodes.Call, genericMethod);
                }
            }

            return null;
        }
    }
}
