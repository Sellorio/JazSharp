using JazSharp.Reflection;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace JazSharp.SpyLogic
{
    public static class SpyExecutionHelper
    {
        private static readonly string[] _assemblyBlacklist =
        {
            "System.Private.CoreLib",
            "System.Threading.Thread"
        };

        public static object HandleCall(long methodPointer, object instance, object[] parameters)
        {
            var handle = (GCHandle)new IntPtr(methodPointer);
            var method = (MethodInfo)handle.Target;

            if (!method.IsStatic && instance == null)
            {
                throw new NullReferenceException();
            }

            var spyInfo = SpyInfo.Get(method);

            if (spyInfo == null)
            {
                return CallThrough(method, instance, parameters);
            }

            var key = spyInfo.Method.IsStatic ? TestScopeHelper.GetTestName() : instance;

            // spy not configured for this instance/static scope
            if (!spyInfo.CallsLog.ContainsKey(key))
            {
                return CallThrough(method, instance, parameters);
            }

            spyInfo.CallsLog[key].Add(parameters);

            var result =
                spyInfo.CallThroughMapping[key]
                    ? CallThrough(method, instance, parameters)
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

        private static object CallThrough(MethodInfo method, object instance, object[] parameters)
        {
            if ((method.MethodImplementationFlags & MethodImplAttributes.InternalCall) == 0     // exclude dllimport functions
                || _assemblyBlacklist.Contains(method.DeclaringType.Assembly.GetName().Name))   // exclude system functions
            {
                return method.Invoke(instance, parameters);
            }
            else
            {
                return InvokationHelper.InvokeMethodWithSpySupport(method, instance, parameters);
            }
        }

        private static object GetDefaultValue(Type type)
        {
            return type == typeof(void) || type.IsClass ? null : Activator.CreateInstance(type);
        }
    }
}
