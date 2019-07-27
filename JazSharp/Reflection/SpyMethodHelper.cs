using JazSharp.SpyLogic;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace JazSharp.Reflection
{
    internal static class SpyMethodHelper
    {
        private static readonly MethodInfo _spyExecutionMethod =
            typeof(SpyExecutionHelper).GetMethod(nameof(SpyExecutionHelper.HandleCall), BindingFlags.Static | BindingFlags.Public);

        private static readonly ConcurrentDictionary<MethodInfo, DynamicMethod> _spyMethodCache =
            new ConcurrentDictionary<MethodInfo, DynamicMethod>();

        internal static DynamicMethod GetSpyMethod(MethodInfo target)
        {
            return _spyMethodCache.GetOrAdd(target, CreateSpyMethod);
        }

        private static DynamicMethod CreateSpyMethod(MethodInfo target)
        {
            var parameters = target.GetParameters();
            var contextParameters = target.IsStatic ? new Type[0] : new[] { target.DeclaringType };

            //var result =
            //    new DynamicMethod(
            //        target.Name,
            //        MethodAttributes.Public | MethodAttributes.Static,
            //        CallingConventions.Standard,
            //        target.ReturnType,
            //        contextParameters.Concat(parameters.Select(x => x.ParameterType)).ToArray(),
            //        DynamicMethodHelper.Module,
            //        true);

            var result =
                new DynamicMethod(
                    target.Name,
                    target.ReturnType,
                    contextParameters.Concat(parameters.Select(x => x.ParameterType)).ToArray());

            var codeGenerator = result.GetILGenerator();

            var argsArrayVariable = codeGenerator.DeclareLocal(typeof(object[]));

            EmitInteger(codeGenerator, parameters.Length);
            codeGenerator.Emit(OpCodes.Newarr);
            codeGenerator.Emit(OpCodes.Stloc_0);

            for (var i = 0; i < parameters.Length; i++)
            {
                codeGenerator.Emit(OpCodes.Ldloc_0);
                EmitInteger(codeGenerator, i);
                EmitParameter(codeGenerator, (byte)i, target);

                if (parameters[i].ParameterType.IsValueType)
                {
                    codeGenerator.Emit(OpCodes.Box);
                }

                codeGenerator.Emit(OpCodes.Stelem, parameters[0].ParameterType.MetadataToken);
            }

            var handle = GCHandle.Alloc(target);
            codeGenerator.Emit(OpCodes.Ldc_I8, ((IntPtr)handle).ToInt64());
            codeGenerator.Emit(target.IsStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0);
            codeGenerator.Emit(OpCodes.Ldloc_0);
            codeGenerator.Emit(OpCodes.Callvirt, _spyExecutionMethod.MetadataToken);
            codeGenerator.Emit(OpCodes.Ldc_I4_0);
            codeGenerator.Emit(OpCodes.Ret);

            DynamicMethodHelper.CreateDelegate(result);

            return result;
        }

        private static void EmitParameter(ILGenerator codeGenerator, byte index, MethodInfo method)
        {
            if (method.IsStatic)
            {
                switch (index)
                {
                    case 0:
                        codeGenerator.Emit(OpCodes.Ldarg_0);
                        break;
                    case 1:
                        codeGenerator.Emit(OpCodes.Ldarg_1);
                        break;
                    case 2:
                        codeGenerator.Emit(OpCodes.Ldarg_2);
                        break;
                    case 3:
                        codeGenerator.Emit(OpCodes.Ldarg_3);
                        break;
                    default:
                        codeGenerator.Emit(OpCodes.Ldarg_S, index);
                        break;
                }
            }
            else
            {
                switch (index)
                {
                    case 0:
                        codeGenerator.Emit(OpCodes.Ldarg_1);
                        break;
                    case 1:
                        codeGenerator.Emit(OpCodes.Ldarg_2);
                        break;
                    case 2:
                        codeGenerator.Emit(OpCodes.Ldarg_3);
                        break;
                    default:
                        codeGenerator.Emit(OpCodes.Ldarg_S, index + 1);
                        break;
                }
            }
        }

        private static void EmitInteger(ILGenerator codeGenerator, int value)
        {
            switch (value)
            {
                case 0:
                    codeGenerator.Emit(OpCodes.Ldc_I4_0);
                    break;
                case 1:
                    codeGenerator.Emit(OpCodes.Ldc_I4_1);
                    break;
                case 2:
                    codeGenerator.Emit(OpCodes.Ldc_I4_2);
                    break;
                case 3:
                    codeGenerator.Emit(OpCodes.Ldc_I4_3);
                    break;
                case 4:
                    codeGenerator.Emit(OpCodes.Ldc_I4_4);
                    break;
                case 5:
                    codeGenerator.Emit(OpCodes.Ldc_I4_5);
                    break;
                case 6:
                    codeGenerator.Emit(OpCodes.Ldc_I4_6);
                    break;
                case 7:
                    codeGenerator.Emit(OpCodes.Ldc_I4_7);
                    break;
                case 8:
                    codeGenerator.Emit(OpCodes.Ldc_I4_8);
                    break;
                default:
                    if (value < 256)
                    {
                        codeGenerator.Emit(OpCodes.Ldc_I4_S, (byte)value);
                    }
                    else
                    {
                        codeGenerator.Emit(OpCodes.Ldc_I4, value);
                    }
                    break;
            }
        }
    }
}
