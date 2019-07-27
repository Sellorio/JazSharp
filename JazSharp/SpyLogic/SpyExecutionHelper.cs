using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace JazSharp.SpyLogic
{
    public static class SpyExecutionHelper
    {
        public static object HandleCall(long methodPointer, object instance, object[] parameters)
        {
            var handle = (GCHandle)new IntPtr(methodPointer);
            var method = (MethodInfo)handle.Target;

            var spyInfo = SpyInfo.Get(method);

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
            return spyInfo.Method.Invoke(instance, parameters);
        }

        private static object GetDefaultValue(Type type)
        {
            return type == typeof(void) || type.IsClass ? null : Activator.CreateInstance(type);
        }
    }
}
