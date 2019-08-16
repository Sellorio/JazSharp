using System;
using System.Linq;

namespace JazSharp
{
    /// <summary>
    /// An exception that is thrown when an expectation fails.
    /// </summary>
    public class JazExpectationException : Exception
    {
        private readonly int _stackTraceTrimAmount;
        private string _trimmedStackTrace;

        /// <summary>
        /// Gets a string representation of the immediate frames on the call stack.
        /// </summary>
        public override string StackTrace =>
            _trimmedStackTrace ??
                (_trimmedStackTrace = GenerateTrimmedStackTrace(base.StackTrace, _stackTraceTrimAmount));

        internal JazExpectationException(string message, int stackTraceRowsToTrim)
            : base(message)
        {
            _stackTraceTrimAmount = stackTraceRowsToTrim;
            
        }

        private static string GenerateTrimmedStackTrace(string originalStackTrace, int trimAmount)
        {
            if (originalStackTrace == null)
            {
                return null;
            }

            var lines = originalStackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(Environment.NewLine, lines.Skip(trimAmount));
        }
    }
}
