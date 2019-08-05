using System;
using System.Collections.Generic;

namespace JazSharp.Testing
{
    internal sealed class RunnableTest : Test
    {
        internal Delegate Execution { get; }

        internal RunnableTest(
            Type testClass,
            IEnumerable<string> path,
            string description,
            Delegate execution,
            bool isFocused,
            bool isExcluded,
            string sourceFilename,
            int lineNumber)
            : base(testClass, path, description, execution, isFocused, isExcluded, sourceFilename, lineNumber)
        {
            Execution = execution;
        }
    }
}
