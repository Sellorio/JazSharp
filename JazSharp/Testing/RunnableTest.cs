using System;
using System.Collections.Generic;

namespace JazSharp.Testing
{
    internal sealed class RunnableTest : Test
    {
        internal Delegate Execution { get; }

        internal RunnableTest(
            IEnumerable<string> path,
            string description,
            Delegate execution,
            bool isFocused,
            bool isExcluded,
            string sourceFilename,
            int lineNumber)
            : base(path, description, execution, isFocused, isExcluded, sourceFilename, lineNumber)
        {
            Execution = execution;
        }
    }
}
