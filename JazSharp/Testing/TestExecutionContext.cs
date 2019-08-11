using System.Text;

namespace JazSharp.Testing
{
    /// <summary>
    /// The context for the current test's execution. This can be used to write custom
    /// messages to the test's output.
    /// </summary>
    public sealed class TestExecutionContext
    {
        /// <summary>
        /// The test that is currently being executed.
        /// </summary>
        public string TestDescription { get; }

        /// <summary>
        /// The output for the test.
        /// </summary>
        public StringBuilder Output { get; } = new StringBuilder();

        internal TestExecutionContext(string testDescription, StringBuilder output)
        {
            Output = output;
            TestDescription = testDescription;
        }
    }
}
