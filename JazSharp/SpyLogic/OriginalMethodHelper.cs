using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace JazSharp.SpyLogic
{
    internal static class OriginalMethodHelper
    {
        internal static MethodInfo GetOrignalMethod(int expectedParameterCount, bool expectingFunc)
        {
            var stackTrace = new StackTrace();
            var callingMethodFrame = stackTrace.GetFrame(3);

            return GetOriginalMethod(callingMethodFrame.GetMethod(), callingMethodFrame.GetILOffset(), expectedParameterCount, expectingFunc);
        }

        private static MethodInfo GetOriginalMethod(MethodBase callingMethod, int callIlOffset, int expectedParameterCount, bool expectingFunc)
        {
            var instructions = GetMethodInstructions(callingMethod);

            var locatedMethod = default(GenericInstanceMethod);
            var currentInstruction = instructions.Last(x => x.Offset < callIlOffset);

            while (locatedMethod == null
                || locatedMethod.Parameters.Count != expectedParameterCount
                || locatedMethod.ReturnType.Name == "Void" == expectingFunc)
            {
                while (currentInstruction.OpCode != OpCodes.Callvirt && currentInstruction.OpCode != OpCodes.Call)
                {
                    if (currentInstruction.Next == null)
                    {
                        return null; // failed to find call (this shouldn't happen)
                    }

                    currentInstruction = currentInstruction.Next;
                }

                locatedMethod = currentInstruction.Operand as GenericInstanceMethod;
            }

            var spyIndex = GetSpyIndex(instructions, currentInstruction);
            var originalToken = GetOriginalMetadataToken(instructions, spyIndex);

            return (MethodInfo)callingMethod.Module.ResolveMethod(originalToken);
        }

        private static Collection<Instruction> GetMethodInstructions(MethodBase method)
        {
            var assemblyDefinition = AssemblyDefinition.ReadAssembly(method.DeclaringType.Assembly.Location);
            var typeDefinition = assemblyDefinition.MainModule.GetType(method.DeclaringType.ToString().Replace("+", "/"));
            var methodDefinition =
                typeDefinition.Methods.Single(x =>
                    x.Name == method.Name &&
                    x.Parameters.Select(y => y.ParameterType.Name).SequenceEqual(method.GetParameters().Select(y => y.ParameterType.Name)));

            return methodDefinition.Body.Instructions;
        }

        private static int GetSpyIndex(Collection<Instruction> methodBody, Instruction currentSpyCall)
        {
            var instructionIndex = methodBody.IndexOf(currentSpyCall);

            return
                methodBody
                    .Take(instructionIndex)
                    .Count(x => (x.OpCode == OpCodes.Callvirt || x.OpCode == OpCodes.Call) && ((MethodReference)x.Operand).DeclaringType.Name == nameof(SpyEntryPoints));
        }

        private static int GetOriginalMetadataToken(Collection<Instruction> methodBody, int spyIndex)
        {
            var originalsTokens = new List<int>();
            var currentInstruction = methodBody.Last().Previous;

            while (currentInstruction.OpCode != OpCodes.Ret)
            {
                if (currentInstruction.OpCode == OpCodes.Ldc_I4)
                {
                    originalsTokens.Insert(0, (int)currentInstruction.Operand);
                }

                currentInstruction = currentInstruction.Previous;
            }

            return originalsTokens[spyIndex];
        }
    }
}
