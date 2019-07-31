using JazSharp.Testing;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System.Collections.Generic;

namespace JazSharp.TestAdapter
{
    [FileExtension(".dll")]
    [FileExtension(".exe")]
    [DefaultExecutorUri(TestAdapterConstants.ExecutorUriString)]
    [ExtensionUri(TestAdapterConstants.ExecutorUriString)]
    public class TestExecutor : ITestExecutor
    {
        public void Cancel()
        {
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            var _testCollection = TestCollection.FromSources(sources);
            var results = _testCollection.CreateTestRun().ExecuteAsync().Result;

            foreach (var result in results)
            {
                frameworkHandle.RecordResult(
                    new TestResult(
                        new TestCase(result.Test.FullName, TestAdapterConstants.ExecutorUri, result.Test.AssemblyFilename)
                        {
                            CodeFilePath = result.Test.SourceFilename,
                            LineNumber = result.Test.LineNumber,
                            DisplayName = result.Test.Description
                        })
                    {
                        Outcome = result.Result,
                        Duration = result.Duration
                    });
            }
        }
    }
}
