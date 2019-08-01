using JazSharp.Testing;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System.Collections.Generic;
using System.Linq;
using VisualStudioTestResult = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult;

namespace JazSharp.TestAdapter
{
    [FileExtension(".dll")]
    [FileExtension(".exe")]
    [DefaultExecutorUri(TestAdapterConstants.ExecutorUriString)]
    [ExtensionUri(TestAdapterConstants.ExecutorUriString)]
    public class TestExecutor : ITestExecutor
    {
        private TestRun _testRun;

        public void Cancel()
        {
            if (_testRun != null)
            {
                _testRun.Cancel();
            }
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            var testsList = tests.ToList();
            var testCollection = TestCollection.FromSources(testsList.Select(x => x.Source).Distinct());

            testCollection.Filter(x => testsList.Any(y => x.IsForTestCase(y)));

            ExecuteTestRun(testCollection, frameworkHandle);
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            var testCollection = TestCollection.FromSources(sources);
            ExecuteTestRun(testCollection, frameworkHandle);
        }

        private void ExecuteTestRun(TestCollection testCollection, IFrameworkHandle frameworkHandle)
        {
            _testRun = testCollection.CreateTestRun();

            _testRun.TestCompleted += result =>
            {
                frameworkHandle.RecordResult(
                    new VisualStudioTestResult(result.Test.ToTestCase())
                    {
                        Outcome = OutcomeFromResult(result.Result),
                        Duration = result.Duration
                    });
            };

            _testRun.ExecuteAsync().GetAwaiter().GetResult();
        }

        private static TestOutcome OutcomeFromResult(Testing.TestResult result)
        {
            switch (result)
            {
                case Testing.TestResult.Passed:
                    return TestOutcome.Passed;
                case Testing.TestResult.Failed:
                    return TestOutcome.Failed;
                case Testing.TestResult.Skipped:
                    return TestOutcome.Skipped;
                default:
                    return TestOutcome.None;
            }
        }
    }
}
