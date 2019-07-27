using System.Reflection.Emit;

namespace JazSharp.Reflection
{
    internal class Instruction
    {
        public OpCode Operation { get; }
        public object Operand { get; }

        internal Instruction(OpCode operation, object operand)
        {
            Operation = operation;
            Operand = operand;
        }
    }
}
