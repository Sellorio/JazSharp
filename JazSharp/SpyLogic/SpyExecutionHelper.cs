using Mono.Cecil;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace JazSharp.SpyLogic
{
    internal static class SpyExecutionHelper
    {
        internal static object HandleCall(MethodBase spyImplementation, object instance, object[] parameters)
        {
            var methodFullName =
                GetCalledMethodFullName(
                    parameters.Length,
                    ((MethodInfo)spyImplementation).ReturnType != typeof(void),
                    spyImplementation.IsStatic);

            var spyInfo = SpyInfo.Get(methodFullName);

            if (spyInfo == null)
            {
                throw new InvalidOperationException(typeof(SpyExecutionHelper).Name + " should only be called from spy methods.");
            }

            var key = spyInfo.Method.IsStatic ? TestScopeHelper.GetTestName() : instance;

            // spy not configured for this instance/static scope
            if (!spyInfo.CallsLog.ContainsKey(key))
            {
                return CallThrough(spyInfo, instance, parameters);
            }

            spyInfo.CallsLog[key].Add(parameters);

            var result =
                spyInfo.CallThroughMapping[key]
                    ? CallThrough(spyInfo, instance, parameters)
                    : HandleReturnValue(spyInfo, key);

            spyInfo.CallbackMapping[key]?.Invoke(parameters);

            return result;
        }

        private static object HandleReturnValue(SpyInfo spyInfo, object key)
        {
            if (spyInfo.ReturnValueMapping.ContainsKey(key))
            {
                return spyInfo.ReturnValueMapping[key];
            }

            if (spyInfo.ReturnValuesMapping.ContainsKey(key))
            {
                var returnValues = spyInfo.ReturnValuesMapping[key];

                if (returnValues.Count == 0)
                {
                    throw new JazSpyException("Unexpected call to spy. Not enough return values have been provided.");
                }

                var result = returnValues.Dequeue();

                return result;
            }

            return GetDefaultValue(spyInfo.Method.ReturnType);
        }

        private static object CallThrough(SpyInfo spyInfo, object instance, object[] parameters)
        {
            spyInfo.Detach();

            try
            {
                return spyInfo.Method.Invoke(instance, parameters);
            }
            finally
            {
                spyInfo.Attach();
            }
        }

        private static object GetDefaultValue(Type type)
        {
            return type == typeof(void) || type.IsClass ? null : Activator.CreateInstance(type);
        }

        private static string GetCalledMethodFullName(int expectedParameterCount, bool expectingFunc, bool expectingStatic)
        {
            var stackTrace = new StackTrace();
            var callingMethodFrame = stackTrace.GetFrames()[3];
            var callingMethod = callingMethodFrame.GetMethod();
            var assemblyDefinition = AssemblyDefinition.ReadAssembly(callingMethod.DeclaringType.Assembly.Location);
            var typeDefinition = assemblyDefinition.MainModule.GetType(callingMethod.DeclaringType.ToString().Replace("+", "/"));
            var methodDefinition =
                typeDefinition.Methods.Single(x =>
                    x.Name == callingMethod.Name &&
                    x.Parameters.Select(y => y.ParameterType.Name).SequenceEqual(callingMethod.GetParameters().Select(y => y.ParameterType.Name)));

            var locatedMethod = default(MethodDefinition);
            var callIl = methodDefinition.Body.Instructions.Last(x => x.Offset < callingMethodFrame.GetILOffset());

            while (locatedMethod == null
                || locatedMethod.Parameters.Count != expectedParameterCount
                || locatedMethod.ReturnType.Name == "Void" == expectingFunc
                || locatedMethod.IsStatic != expectingStatic)
            {
                while (callIl.OpCode.Name != OpCodes.Callvirt.Name && callIl.OpCode.Name != OpCodes.Call.Name)
                {
                    if (callIl.Next == null)
                    {
                        return string.Empty; // failed to find call (this shouldn't happen)
                    }

                    callIl = callIl.Next;
                }

                locatedMethod = (MethodDefinition)callIl.Operand;
            }

            return locatedMethod.FullName;
        }
    }
}
