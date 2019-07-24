using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace JazSharp.Emit
{
    internal static class Emitter
    {
        private static uint _uniqueKey = 0;

        internal static MethodInfo CreateMethod(MethodInfo basedOn)
        {
            var dynamicMethod = new DynamicMethod(basedOn.Name, basedOn.ReturnType, basedOn.GetParameters().Select(x => x.ParameterType).ToArray());
            dynamicMethod.GetILGenerator().Emit(OpCodes.Ret);
            return dynamicMethod.CreateDelegate(typeof(Func<object>)).Method; // temporary
        }
    }
}
