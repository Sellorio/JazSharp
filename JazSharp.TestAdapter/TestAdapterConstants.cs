using System;

namespace JazSharp.TestAdapter
{
    internal static class TestAdapterConstants
    {
        internal const string ExecutorUriString = "executor://jazsharp/VsTestRunner2/net";
        internal static readonly Uri ExecutorUri = new Uri(ExecutorUriString);
    }
}
