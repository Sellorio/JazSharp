using JazSharp.TestSetup;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;

namespace JazSharp.TestAdapter
{
    [FileExtension(".dll")]
    [FileExtension(".exe")]
    [DefaultExecutorUri(TestAdapterConstants.ExecutorUriString)]
    [Export(typeof(ITestDiscoverer))]
    public sealed class TestDiscoverer : ITestDiscoverer
    {
        private List<Test> _tests;

        public Uri ExecutorUri => TestAdapterConstants.ExecutorUri;

        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            File.WriteAllText(@"C:\Users\seamillo\Desktop\TestDisc.txt", "Success");
            _tests =
                sources
                    .Select(Assembly.LoadFrom)
                    .SelectMany(x => x.GetTypes())
                    .Where(x => x.IsAssignableFrom(typeof(Spec)) && !x.IsAbstract && x.GetConstructor(new Type[0]) != null)
                    .SelectMany(SpecHelper.GetTestsInSpec)
                    .ToList();

            foreach (var test in _tests)
            {
                discoverySink.SendTestCase(
                    new TestCase(test.FullName, ExecutorUri, test.Execution.Method.DeclaringType.Assembly.Location)
                    {
                        DisplayName = test.FullName
                    });
            }
        }
    }
}
