using JazSharp.Spies;
using System;
using System.Linq;

namespace JazSharp.SpyLogic
{
    internal static class SpyExecutionHelper
    {
        internal static object HandleCall(object[] parameters, string serializedMethodCallInfo)
        {
            var method = OriginalMethodHelper.GetMethod(serializedMethodCallInfo);

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

            var key = method.IsStatic ? string.Empty : instance;
            var spy = Spy.Get(method, key);

            if (spy == null)
            {
                return method.Invoke(instance, parameters);
            }

            spy.CallLog.Add(parameters);

            if (!spy.Behaviours.Any())
            {
                throw new JazSpyException("Unexpected call to spy after last behaviour/return value.");
            }

            return spy.Behaviours.Peek().Execute(spy, instance, parameters);
        }
    }
}
