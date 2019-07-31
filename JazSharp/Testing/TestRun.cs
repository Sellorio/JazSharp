using JazSharp.Reflection;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace JazSharp.Testing
{
    /// <summary>
    /// Represents a test execution plan. <see cref="ExecuteAsync"/> can be called to execute
    /// the tests.
    /// </summary>
    public sealed class TestRun : IDisposable
    {
        private readonly IEnumerable<string> _temporaryDirectories;
        private bool _executeInParallel;
        private bool _isExecuting;

        /// <summary>
        /// The tests planned for execution.
        /// </summary>
        public ImmutableArray<Test> Tests { get; }

        /// <summary>
        /// An event that is triggered when one of the tests has completed. This will
        /// not trigger on the same thread as the call to <see cref="ExecuteAsync"/>.
        /// </summary>
        public event Action<TestResultInfo> TestCompleted;

        /// <summary>
        /// An event that is triggered when all tests have completed. This will not trigger
        /// on the same thread as the call to <see cref="ExecuteAsync"/>.
        /// </summary>
        public event Action TestRunCompleted;

        /// <summary>
        /// Whether or not to execute the tests in parallel.
        /// </summary>
        public bool ExecuteInParallel
        {
            get => _executeInParallel;
            set
            {
                if (_isExecuting)
                {
                    throw new InvalidOperationException("Cannot change test run settings while tests are executing.");
                }

                _executeInParallel = value;
            }
        }

        private TestRun(IEnumerable<RunnableTest> tests, IEnumerable<string> temporaryDirectories)
        {
            _temporaryDirectories = temporaryDirectories;
            Tests = ImmutableArray.CreateRange<Test>(tests);
        }

        /// <summary>
        /// Executes the test run and returns the results when awaited.
        /// </summary>
        /// <returns>A task to retrieve the results of the test run.</returns>
        public Task<TestResultInfo[]> ExecuteAsync()
        {
            if (_isExecuting)
            {
                throw new InvalidOperationException("A test run can only be executed once at a time.");
            }

            _isExecuting = true;

            return ExecuteInParallel ? ExecuteParallelAsync() : ExecuteSequenceAsync();
        }

        /// <summary>
        /// Disposes the test run by deleting the temporary shadow-copy directories that were created.
        /// </summary>
        public void Dispose()
        {
            foreach (var directory in _temporaryDirectories)
            {
                Directory.Delete(directory, true);
            }
        }

        private Task<TestResultInfo[]> ExecuteSequenceAsync()
        {
            return Task.Run(async () =>
            {
                List<TestResultInfo> results = new List<TestResultInfo>();

                foreach (RunnableTest test in Tests)
                {
                    var result =
                        test.Execution is Action action
                            ? await TestExecutionAsync(action)
                            : await TestExecutionAsync((Func<Task>)test.Execution);

                    results.Add(result);
                }

                TestRunCompleted?.Invoke();

                return results.ToArray();
            });
        }

        private Task<TestResultInfo[]> ExecuteParallelAsync()
        {
            var tasks =
                Tests.Cast<RunnableTest>().Select(x =>
                    x.Execution is Action action
                        ? TestExecutionAsync(action)
                        : TestExecutionAsync((Func<Task>)x.Execution));

            var result = Task.WhenAll(tasks);
            result.ContinueWith(_ => TestRunCompleted?.Invoke());

            return result;
        }

        private Task<TestResultInfo> TestExecutionAsync(Action test)
        {
            return TestExecutionAsync(() =>
            {
                test();
                return Task.CompletedTask;
            });
        }

        private async Task<TestResultInfo> TestExecutionAsync(Func<Task> test)
        {
            throw new NotImplementedException();
            //TestCompleted?.Invoke(result);
        }

        /// <summary>
        /// Creates a test run from a test collection - using the current tests in the collection
        /// as the source of tests to be executed. Changing the test collection after this call
        /// will not affect the created test run.
        /// </summary>
        /// <param name="testCollection">The test collection providing tests for the run.</param>
        /// <returns>The created test run.</returns>
        public static TestRun FromCollection(TestCollection testCollection)
        {
            if (testCollection == null)
            {
                throw new ArgumentNullException(nameof(testCollection));
            }

            var sourcesGroupedByDirectory = testCollection.Sources.GroupBy(Path.GetDirectoryName).Distinct();
            var sources = new List<string>();
            var temporaryDirectories = new List<string>();

            foreach (var directory in sourcesGroupedByDirectory)
            {
                var shadowcopyTargetDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

                if (Directory.Exists(shadowcopyTargetDirectory))
                {
                    Directory.Delete(shadowcopyTargetDirectory, true);
                }

                CopyDirectory(directory.Key, shadowcopyTargetDirectory);
                temporaryDirectories.Add(shadowcopyTargetDirectory);

                foreach (var source in directory)
                {
                    var newSourcePath = source.Replace(directory.Key, shadowcopyTargetDirectory);
                    sources.Add(newSourcePath);
                    AssemblyRewriteHelper.RewriteAssembly(newSourcePath, testCollection);
                }
            }

            var executionReadyAssemblies = sources.Select(Assembly.LoadFrom).ToList();

            return
                new TestRun(
                    testCollection.Tests.Select(x => x.Prepare(executionReadyAssemblies.First(y => y.FullName == x.AssemblyName))),
                    temporaryDirectories);
        }

        private static void CopyDirectory(string source, string destination)
        {
            foreach (string dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(source, destination));
            }

            foreach (string newPath in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(source, destination), true);
            }
        }
    }
}
