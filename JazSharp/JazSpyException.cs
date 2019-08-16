using System;

namespace JazSharp
{
    /// <summary>
    /// An expection that may be thrown when defining a spy or executing a spy's logic.
    /// </summary>
    public class JazSpyException : Exception
    {
        internal JazSpyException(string message)
            : base(message)
        {
        }
    }
}
