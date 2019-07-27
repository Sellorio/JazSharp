using System.Collections.Generic;
using System.Reflection;

namespace JazSharp.Reflection
{
    internal class MethodBody
    {
        public Instruction[] Instructions { get; }
        public IList<LocalVariableInfo> LocalVariables { get; }

        internal MethodBody(Instruction[] instructions, IList<LocalVariableInfo> localVariables)
        {
            Instructions = instructions;
            LocalVariables = localVariables;
        }
    }
}
