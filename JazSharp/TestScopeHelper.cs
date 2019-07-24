using System.Diagnostics;
using System.Linq;

namespace JazSharp
{
    internal static class TestScopeHelper
    {
        internal static string GetTestName()
        {
            var stackTrace = new StackTrace();
            var frames = stackTrace.GetFrames();
            var testMethodFrame = frames.LastOrDefault(x => !string.IsNullOrEmpty(x.GetFileName())) ?? frames.Last();

            return testMethodFrame.GetMethod().Name;
        }
    }
}
