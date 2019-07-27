using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace JazSharp.Reflection
{
    internal static class DynamicMethodHelper
    {
        internal static Module Module { get; }

        static DynamicMethodHelper()
        {
            var ass = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("JazDynamics"), AssemblyBuilderAccess.Run);
            Module = ass.DefineDynamicModule("Main");
        }

        internal static Delegate CreateDelegate(DynamicMethod dynamicMethod)
        {
            var parameterTypes = dynamicMethod.GetParameters().Select(x => x.ParameterType).ToArray();
            Type delegateType = null;

            if (dynamicMethod.ReturnType == typeof(void))
            {
                switch (parameterTypes.Length)
                {
                    case 0:
                        delegateType = typeof(Action);
                        break;
                    case 1:
                        delegateType = typeof(Action<>).MakeGenericType(parameterTypes);
                        break;
                    case 2:
                        delegateType = typeof(Action<,>).MakeGenericType(parameterTypes);
                        break;
                    case 3:
                        delegateType = typeof(Action<,,>).MakeGenericType(parameterTypes);
                        break;
                    case 4:
                        delegateType = typeof(Action<,,,>).MakeGenericType(parameterTypes);
                        break;
                    case 5:
                        delegateType = typeof(Action<,,,,>).MakeGenericType(parameterTypes);
                        break;
                    case 6:
                        delegateType = typeof(Action<,,,,,>).MakeGenericType(parameterTypes);
                        break;
                    case 7:
                        delegateType = typeof(Action<,,,,,,>).MakeGenericType(parameterTypes);
                        break;
                    case 8:
                        delegateType = typeof(Action<,,,,,,,>).MakeGenericType(parameterTypes);
                        break;
                }
            }
            else
            {
                var allTypes = parameterTypes.Concat(new[] { dynamicMethod.ReturnType }).ToArray();

                switch (parameterTypes.Length)
                {
                    case 0:
                        delegateType = typeof(Func<>);
                        break;
                    case 1:
                        delegateType = typeof(Func<,>).MakeGenericType(allTypes);
                        break;
                    case 2:
                        delegateType = typeof(Func<,,>).MakeGenericType(allTypes);
                        break;
                    case 3:
                        delegateType = typeof(Func<,,,>).MakeGenericType(allTypes);
                        break;
                    case 4:
                        delegateType = typeof(Func<,,,,>).MakeGenericType(allTypes);
                        break;
                    case 5:
                        delegateType = typeof(Func<,,,,,>).MakeGenericType(allTypes);
                        break;
                    case 6:
                        delegateType = typeof(Func<,,,,,,>).MakeGenericType(allTypes);
                        break;
                    case 7:
                        delegateType = typeof(Func<,,,,,,,>).MakeGenericType(allTypes);
                        break;
                    case 8:
                        delegateType = typeof(Func<,,,,,,,,>).MakeGenericType(allTypes);
                        break;
                }
            }

            return dynamicMethod.CreateDelegate(delegateType);
        }
    }
}
