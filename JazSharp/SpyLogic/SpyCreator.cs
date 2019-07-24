using System;
using System.Collections.Generic;
using System.Reflection;

namespace JazSharp.SpyLogic
{
    internal static class SpyCreator
    {
        internal static SpyInfo CreateSpy(MethodInfo method, object key)
        {
            if (method.DeclaringType.Namespace.StartsWith("System."))
            {
                throw new InvalidOperationException("Spying on system methods is not permitted.");
            }

            var spy = SpyInfo.Get(method);

            if (spy == null)
            {
                spy = SpyInfo.Create(method);
                spy.Attach();
            }

            spy.CallsLog[key] = new List<object[]>();
            spy.CallThroughMapping[key] = false;
            spy.CallbackMapping[key] = null;

            if (spy.ReturnValueMapping.ContainsKey(key))
            {
                spy.ReturnValueMapping.Remove(key);
            }

            if (spy.ReturnValuesMapping.ContainsKey(key))
            {
                spy.ReturnValuesMapping.Remove(key);
            }

            return spy;
        }
    }
}
