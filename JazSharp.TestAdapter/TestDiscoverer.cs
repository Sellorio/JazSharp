using JazSharp.Testing;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
namespace JazSharp.TestAdapter
{
    [FileExtension(".dll")]
    [FileExtension(".exe")]
    [DefaultExecutorUri(TestAdapterConstants.ExecutorUriString)]
    [Export(typeof(ITestDiscoverer))]
    public sealed class TestDiscoverer : ITestDiscoverer, IDisposable
    {
        private TestCollection _testCollection;

        public Uri ExecutorUri => TestAdapterConstants.ExecutorUri;

        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            Dispose();

            File.WriteAllText(@"C:\Users\seamillo\Desktop\TestDisc.txt", "Success");
            _testCollection = TestCollection.FromSources(sources);

            foreach (var test in _testCollection.Tests)
            {
                discoverySink.SendTestCase(
                    new TestCase(test.FullName, ExecutorUri, test.AssemblyFilename)
                    {
                        CodeFilePath = test.SourceFilename,
                        LineNumber = test.LineNumber,
                        DisplayName = test.Description
                    });
            }
        }

        public void Dispose()
        {
            if (_testCollection != null)
            {
                _testCollection.Dispose();
                _testCollection = null;
            }
        }
    }
}
