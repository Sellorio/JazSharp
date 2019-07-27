using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace JazSharp.Reflection
{
    internal static class InvokationHelper
    {
        private static readonly ConcurrentDictionary<MethodInfo, DynamicMethod> _wrappedMethodCache = new ConcurrentDictionary<MethodInfo, DynamicMethod>();

        internal static object InvokeMethodWithSpySupport(MethodInfo method, object obj, object[] parameters)
        {
            var methodToCall = GetSpyEnabledCopy(method);
            return methodToCall.Invoke(null, new[] { obj }.Concat(parameters).ToArray());
        }

        private static DynamicMethod GetSpyEnabledCopy(MethodInfo method)
        {
            return _wrappedMethodCache.GetOrAdd(method, CreateSpyEnabledCopy);
        }

        private static DynamicMethod CreateSpyEnabledCopy(MethodInfo method)
        {
            var originalMethod = IntermediateLanguageHelper.ParseBody(method);
            var contextParaeters = method.IsStatic ? new Type[0] : new[] { method.DeclaringType };

            //var result =
            //    new DynamicMethod(
            //        method.Name,
            //        MethodAttributes.Public | MethodAttributes.Static,
            //        CallingConventions.Standard,
            //        method.ReturnType,
            //        contextParaeters.Concat(method.GetParameters().Select(x => x.ParameterType)).ToArray(),
            //        DynamicMethodHelper.Module,
            //        true);

            var result =
                new DynamicMethod(
                    method.Name,
                    method.ReturnType,
                    contextParaeters.Concat(method.GetParameters().Select(x => x.ParameterType)).ToArray());

            var codeGenerator = result.GetILGenerator();

            foreach (var originalLocal in originalMethod.LocalVariables.OrderBy(x => x.LocalIndex))
            {
                codeGenerator.DeclareLocal(originalLocal.LocalType, originalLocal.IsPinned);
            }

            foreach (var instruction in originalMethod.Instructions)
            {
                var outputInstruction = instruction;

                if (instruction.Operation.OperandType == OperandType.InlineMethod)
                {
                    var spyMethod = SpyMethodHelper.GetSpyMethod((MethodInfo)instruction.Operand);
                    outputInstruction = new Instruction(instruction.Operation, spyMethod);
                }

                switch (outputInstruction.Operand)
                {
                    case null:
                        codeGenerator.Emit(outputInstruction.Operation);
                        break;
                    case MethodInfo methodInfo:
                        codeGenerator.Emit(OpCodes.Call, methodInfo);
                        break;
                    case FieldInfo fieldInfo:
                        codeGenerator.Emit(outputInstruction.Operation, fieldInfo);
                        break;
                    case string stringValue:
                        codeGenerator.Emit(outputInstruction.Operation, stringValue);
                        break;
                    case Type type:
                        codeGenerator.Emit(outputInstruction.Operation, type);
                        break;
                    case long doubleOrLong:
                        codeGenerator.Emit(outputInstruction.Operation, doubleOrLong);
                        break;
                    case float floatValue:
                        codeGenerator.Emit(outputInstruction.Operation, floatValue);
                        break;
                    case double doubleValue:
                        codeGenerator.Emit(outputInstruction.Operation, doubleValue);
                        break;
                    case byte byteValue:
                        codeGenerator.Emit(outputInstruction.Operation, byteValue);
                        break;
                    case int intAndOthers:
                        codeGenerator.Emit(outputInstruction.Operation, intAndOthers);
                        break;
                    default:
                        throw new NotSupportedException("IL instruction is not supported.");
                }
            }

            DynamicMethodHelper.CreateDelegate(result);

            return result;
        }
    }
}
