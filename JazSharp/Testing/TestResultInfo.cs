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
        /// The result. <see langword="true" /> is success, <see langword="false"/> is failure
        /// and <see langword="null"/> means the test was skipped.
        /// </summary>
        public bool? Result { get; }

        /// <summary>
        /// The output of the test. This will contain exceptions that were thrown as well as
        /// other messages outputted for the test.
        /// </summary>
        public string Output { get; }

        internal TestResultInfo(Test test, bool? result, string output)
        {
            Test = test;
            Result = result;
            Output = output;
        }
    }
}
