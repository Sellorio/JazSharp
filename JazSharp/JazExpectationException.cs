using System;

namespace JazSharp
{
    public class JazExpectationException : Exception
    {
        public JazExpectationException(string message)
            : base(message)
        {
        }
    }
}
