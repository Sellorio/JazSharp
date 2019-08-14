using System;
using System.Linq;

namespace JazSharp
{
    public class JazExpectationException : Exception
    {
        private readonly int _stackTraceTrimAmount;
        private string _trimmedStackTrace;

        public override string StackTrace =>
            _trimmedStackTrace ??
                (_trimmedStackTrace = GenerateTrimmedStackTrace(base.StackTrace, _stackTraceTrimAmount));

        public JazExpectationException(string message, int stackTraceRowsToTrim)
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
