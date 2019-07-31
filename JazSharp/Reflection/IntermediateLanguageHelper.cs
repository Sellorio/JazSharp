using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace JazSharp.Reflection
{
    internal static class IntermediateLanguageHelper
    {
        private static readonly Dictionary<short, OpCode> _opCodes =
            typeof(OpCodes).GetFields().Select(x => (OpCode)x.GetValue(null)).ToDictionary(x => x.Value, x => x);

        internal static MethodBody ParseBody(MethodBase method)
        {
            var methodBody = method.GetMethodBody();
            var instructions = GetMethodInstructions(method.Module, methodBody.GetILAsByteArray());

            return new MethodBody(instructions.ToArray(), methodBody.LocalVariables);
        }

        internal static Instruction2[] GetMethodInstructions(Module module, byte[] binaryIl)
        {
            var instructions = new List<Instruction2>();

            for (var i = 0; i < binaryIl.Length;)
            {
                var operation = _opCodes[binaryIl[i]];
                i++;

                object operand;

                switch (operation.OperandType)
                {
                    case OperandType.InlineNone:
                        operand = null;
                        break;
                    case OperandType.InlineMethod:
                        operand = ResolvePointer(module.ResolveMethod, binaryIl, ref i);
                        break;
                    case OperandType.InlineField:
                        operand = ResolvePointer(module.ResolveField, binaryIl, ref i);
                        break;
                    case OperandType.InlineString:
                        operand = ResolvePointer(module.ResolveString, binaryIl, ref i);
                        break;
                    case OperandType.InlineType:
                        operand = ResolvePointer(module.ResolveType, binaryIl, ref i);
                        break;
                    case OperandType.InlineI: // inline integer
                    case OperandType.InlineBrTarget: // goto for break statement
                    case OperandType.InlineSwitch:
                    case OperandType.InlineSig:
                        operand = BitConverter.ToInt32(binaryIl, i);
                        i += 4;
                        break;
                    case OperandType.ShortInlineR: // inline float
                        operand = BitConverter.ToSingle(binaryIl, i);
                        i += 4;
                        break;
                    case OperandType.InlineI8: // inline long
                        operand = BitConverter.ToInt64(binaryIl, i);
                        i += 8;
                        break;
                    case OperandType.InlineR: // inline double
                        operand = BitConverter.ToDouble(binaryIl, i);
                        i += 8;
                        break;
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.ShortInlineI:
                    case OperandType.ShortInlineVar:
                        operand = binaryIl[i];
                        i++;
                        break;
                    default:
                        operand = BitConverter.ToInt32(binaryIl, i);
                        i += 4;
                        break;
                }

                instructions.Add(new Instruction2(operation, operand));
            }

            return instructions.ToArray();
        }

        private static TResult ResolvePointer<TResult>(Func<int, TResult> resolveFunc, byte[] code, ref int i)
        {
            var result = resolveFunc(BitConverter.ToInt32(code, i));
            i += 4;
            return result;
        }
    }
}
