using System;
using System.Linq;

namespace JazSharp.SpyLogic
{
    internal static class SpyExecutionHelper
    {
        internal static object HandleCall(object[] parameters, bool isFunc)
        {
            var method = OriginalMethodHelper.GetOrignalMethod(parameters, isFunc);

            if (method == null)
            {
                throw new JazSpyException("JazSharp failed to determine the original method called before spy wrappers were applied.");
            }

            object instance = null;

            if (!method.IsStatic)
            {
                instance = parameters[0];
                parameters = parameters.Skip(1).ToArray();

                if (instance == null)
                {
                    throw new NullReferenceException();
                }
            }

            var spyInfo = SpyInfo.Get(method);

            if (spyInfo == null)
            {
                return method.Invoke(instance, parameters);
            }

            var key = spyInfo.Method.IsStatic ? string.Empty : instance;

            // spy not configured for this instance/static scope
            if (!spyInfo.CallsLog.ContainsKey(key))
            {
                return method.Invoke(instance, parameters);
            }

            spyInfo.CallsLog[key].Add(parameters);

            if (spyInfo.ThrowMapping.TryGetValue(key, out var exceptionToThrow))
            {
                throw exceptionToThrow;
            }

            var result =
                spyInfo.CallThroughMapping[key]
                    ? method.Invoke(instance, parameters)
                    : HandleReturnValue(spyInfo, key);

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

        private static object GetDefaultValue(Type type)
        {
            return type == typeof(void) || type.IsClass ? null : Activator.CreateInstance(type);
        }
    }
}
