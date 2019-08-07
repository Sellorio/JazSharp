using JazSharp.TestAdapter;
using JazSharp.Testing;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace JazSharp.ManualTest
{
    class Program
    {
        static void Main(string[] _)
        {
            //using (var testCollection = TestCollection.FromSources(new[] { @"C:\Users\seamillo\source\repos\JazSharp\JazSharp.Tests\bin\Debug\netcoreapp2.2\JazSharp.Tests.dll" }))
            //using (var testRun = testCollection.CreateTestRun())
            //{
            //    var result = testRun.ExecuteAsync().Result;
            //}

            var disco = new TestDiscoverer();
            var sink = new MockSink();
            disco.DiscoverTests(new[] { @"C:\Users\seamillo\source\repos\JazSharp\JazSharp.Tests\bin\Debug\netcoreapp2.2\JazSharp.Tests.dll" }, null, new MockLogger(), sink);
            var serialized = JsonConvert.SerializeObject(sink.TestCases);
        }

        private class MockLogger : IMessageLogger
        {
            public void SendMessage(TestMessageLevel testMessageLevel, string message)
            {
            }
        }

        private class MockSink : ITestCaseDiscoverySink
        {
            public List<TestCase> TestCases { get; } = new List<TestCase>();

            public void SendTestCase(TestCase discoveredTest)
            {
                TestCases.Add(discoveredTest);
            }
        }
    }
}