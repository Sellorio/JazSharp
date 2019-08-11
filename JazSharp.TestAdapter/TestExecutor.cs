using JazSharp.Testing;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            var testMapping =
                new Dictionary<Test, TestCase>(
                    testCollection.Tests
                        .Select(x => new KeyValuePair<Test, TestCase>(x, testsList.FirstOrDefault(y => x.IsForTestCase(y))))
                        .Where(x => x.Value != null));

            testCollection.Filter(testMapping.ContainsKey);

            ExecuteTestRun(testCollection, frameworkHandle, testMapping);
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            var testCollection = TestCollection.FromSources(sources);
            ExecuteTestRun(testCollection, frameworkHandle, testCollection.Tests.ToDictionary(x => x, x => x.ToTestCase()));
        }

        private void ExecuteTestRun(TestCollection testCollection, IFrameworkHandle frameworkHandle, Dictionary<Test, TestCase> testMapping)
        {
            _testRun = testCollection.CreateTestRun();

            _testRun.TestCompleted += result =>
            {
                var vsResult = new VisualStudioTestResult(testMapping[result.Test])
                {
                    Outcome = OutcomeFromResult(result.Result),
                    Duration = result.Duration,
                    ErrorMessage = result.Output,
                    ErrorStackTrace = GetCombinedStackTrace(result.Exception)
                };

                vsResult.Messages.Add(new TestResultMessage(TestResultMessage.StandardOutCategory, result.Output));
                vsResult.ErrorStackTrace = GetCombinedStackTrace(result.Exception);

                frameworkHandle.RecordResult(vsResult);
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

        private static string GetCombinedStackTrace(Exception exception)
        {
            if (exception == null)
            {
                return null;
            }

            var traces = new List<string>();

            while (exception != null)
            {
                traces.Add(exception.StackTrace);
                exception = exception.InnerException;
            }

            return
                string.Join(
                    Environment.NewLine + "----- Inner Stack Trace -----" + Environment.NewLine,
                    Enumerable.Reverse(traces));
        }
    }
}
