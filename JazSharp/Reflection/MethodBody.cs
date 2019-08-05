using System.Collections.Generic;
using System.Reflection;

namespace JazSharp.Reflection
{
    internal class MethodBody
    {
        public Instruction2[] Instructions { get; }
        public IList<LocalVariableInfo> LocalVariables { get; }

        internal MethodBody(Instruction2[] instructions, IList<LocalVariableInfo> localVariables)
        {
            Instructions = instructions;
            LocalVariables = localVariables;
        }
    }
}
