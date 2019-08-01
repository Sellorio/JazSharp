using System;

namespace JazSharp.Testing
{
    /// <summary>
    /// The results from a test execution.
    /// </summary>
    public sealed class TestResultInfo
    {
        /// <summary>
        /// The test that was executed.
        /// </summary>
        public Test Test { get; }

        /// <summary>
        /// The result.
        /// </summary>
        public TestResult Result { get; }

        /// <summary>
        /// The output of the test. This will contain exceptions that were thrown as well as
        /// other messages outputted for the test.
        /// </summary>
        public string Output { get; }

        /// <summary>
        /// How long in the test took to run.
        /// </summary>
        public TimeSpan Duration { get; }

        internal TestResultInfo(Test test, TestResult result, string output, TimeSpan duration)
        {
            Test = test;
            Result = result;
            Output = output;
            Duration = duration;
        }
    }
}
