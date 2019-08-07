using JazSharp.Testing;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace JazSharp.SpyLogic
{
    internal static class OriginalMethodHelper
    {
        internal static MethodInfo GetOrignalMethod(object[] parameters, bool expectingFunc)
        {
            var stackTrace = new StackTrace(true);
            var callingMethodFrame = stackTrace.GetFrame(3);

            return GetOriginalMethod(callingMethodFrame.GetMethod(), callingMethodFrame.GetILOffset(), parameters, expectingFunc);
        }

        private static MethodInfo GetOriginalMethod(MethodBase callingMethod, int callIlOffset, object[] parameters, bool expectingFunc)
        {
            var instructions = GetMethodInstructions(callingMethod);

            var locatedMethod = default(GenericInstanceMethod);
            var currentInstruction = instructions.FirstOrDefault(x => x.Offset == callIlOffset) ?? instructions.First();

            while (true)
            {
                if (currentInstruction.OpCode == OpCodes.Callvirt || currentInstruction.OpCode == OpCodes.Call)
                {
                    locatedMethod = currentInstruction.Operand as GenericInstanceMethod;
                }

                if (locatedMethod != null
                    && locatedMethod.Parameters.Count == parameters.Length
                    && locatedMethod.ReturnType.Name == "Void" != expectingFunc)
                {
                    break;
                }

                if (currentInstruction.Next == null)
                {
                    throw new JazSpyException("Unable to resolve original method call from spy.");
                }

                currentInstruction = currentInstruction.Next;
            }

            var spyIndex = GetSpyIndex(instructions, currentInstruction);
            var serializedCallInfo = GetOriginalMetadataToken(instructions, spyIndex);

            return GetMethod(serializedCallInfo);
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

        private static string GetOriginalMetadataToken(Collection<Instruction> methodBody, int spyIndex)
        {
            var originalsTokens = new List<string>();
            var currentInstruction = methodBody.Last().Previous;

            while (currentInstruction.OpCode != OpCodes.Ret)
            {
                if (currentInstruction.OpCode == OpCodes.Ldstr)
                {
                    originalsTokens.Insert(0, (string)currentInstruction.Operand);
                }

                currentInstruction = currentInstruction.Previous;
            }

            return originalsTokens[spyIndex];
        }

        private static MethodInfo GetMethod(string serializedInfo)
        {
            var path = serializedInfo.Split(':');
            var assembly = GetAssembly(path[0]);

            var typeInfo = path[1];
            var indexOfGeneric = typeInfo.IndexOf('?');
            var typeName = indexOfGeneric == -1 ? typeInfo : typeInfo.Substring(0, indexOfGeneric);
            var genericTypeArgs = indexOfGeneric == -1 ? null : typeInfo.Substring(indexOfGeneric + 1).Split(' ').Select(GetType).ToArray();
            var type = assembly.GetType(typeName);

            if (type.IsGenericTypeDefinition)
            {
                type = type.MakeGenericType(genericTypeArgs);
            }

            var methodInfo = path[2];
            indexOfGeneric = methodInfo.IndexOf('?');
            var methodDescription = indexOfGeneric == -1 ? methodInfo : methodInfo.Substring(0, indexOfGeneric);
            var genericMethodArgs = indexOfGeneric == -1 ? null : methodInfo.Substring(indexOfGeneric + 1).Split(' ').Select(GetType).ToArray();
            var method =
                type
                    .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                    .First(x => x.ToString() == methodDescription);

            if (method.IsGenericMethodDefinition)
            {
                method = method.MakeGenericMethod(genericMethodArgs);
            }

            return method;
        }

        private static Type GetType(string serializedInfo)
        {
            var parts = serializedInfo.Split('/');
            var assembly = GetAssembly(parts[0]);
            return assembly.GetType(parts[1]);
        }

        private static Assembly GetAssembly(string name)
        {
            var result =
                AssemblyContext.Current.LoadedAssemblies.Values.FirstOrDefault(x => x.GetName().Name == name)
                    ?? AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name == name)
                    ?? AssemblyContext.Current.LoadByName(name)
                    ?? Assembly.Load(name); // should only be needed to load system assemblies

            if (result == null)
            {
                throw new JazSpyException("Unable to resolve original method call from spy.");
            }

            return result;
        }
    }
}
