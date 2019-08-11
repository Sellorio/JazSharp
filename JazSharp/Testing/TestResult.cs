namespace JazSharp.Testing
{
    /// <summary>
    /// The result of the test's execution.
    /// </summary>
    public enum TestResult
    {
        /// <summary>
        /// The test completed without issues.
        /// </summary>
        Passed,
        /// <summary>
        /// The test failed.
        /// </summary>
        Failed,
        /// <summary>
        /// The test was skipped.
        /// </summary>
        Skipped
    }
}
