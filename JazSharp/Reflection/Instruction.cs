using System.Reflection.Emit;

namespace JazSharp.Reflection
{
    internal class Instruction2
    {
        public OpCode Operation { get; }
        public object Operand { get; }

        internal Instruction2(OpCode operation, object operand)
        {
            Operation = operation;
            Operand = operand;
        }
    }
}
