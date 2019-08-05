using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.Loader;

namespace JazSharp.Testing
{
    /// <summary>
    /// Contains a collection of tests that can be run. Should be disposed to ensure that the
    /// temporary <see cref="AppDomain"/> containing the test assemblies is unloaded.
    /// </summary>
    public sealed class TestCollection : IDisposable
    {
        private bool _disposed;
        private readonly AssemblyContext _assemblyContext;

        /// <summary>
        /// The test sources (assembly file names).
        /// </summary>
        public ImmutableArray<string> Sources { get; }

        /// <summary>
        /// The tests that were discovered.
        /// </summary>
        public ImmutableArray<Test> Tests { get; private set; }

        private TestCollection(IEnumerable<string> sources, AssemblyContext assemblyContext, IEnumerable<Test> tests)
        {
            _assemblyContext = assemblyContext;
            Sources = ImmutableArray.CreateRange(sources);
            Tests = ImmutableArray.CreateRange(tests);
        }

        /// <summary>
        /// Filters the tests in the test collection. This is used for partial test runs.
        /// </summary>
        /// <param name="filter">The filter to apply.</param>
        public void Filter(Func<Test, bool> filter)
        {
            Tests = ImmutableArray.CreateRange(Tests.Where(filter));
        }

        /// <summary>
        /// Creates a test run from the tests in the collection.
        /// </summary>
        public TestRun CreateTestRun()
        {
            return TestRun.FromCollection(this);
        }

        /// <summary>
        /// Unloads the <see cref="AppDomain"/> containing the tests.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(typeof(TestCollection).Name);
            }

            //TODO: Unload assembly context here when supported
        }

        /// <summary>
        /// Discovers the tests contained within the provided sources.
        /// </summary>
        /// <param name="sources">The filenames for managed dll or exe files that need to have tests executed against them.</param>
        /// <returns>The discovered tests.</returns>
        public static TestCollection FromSources(IEnumerable<string> sources)
        {
            var sourcesAsList = sources.ToList();

            var assemblyContext = new AssemblyContext();
            IEnumerable<Test> tests;

            try
            {
                tests =
                    sourcesAsList
                        .Where(File.Exists)
                        .Select(assemblyContext.Load)
                        .SelectMany(x => x.GetTypes())
                        .Where(x => typeof(Spec).IsAssignableFrom(x) && !x.IsAbstract && x.GetConstructor(new Type[0]) != null)
                        .SelectMany(SpecHelper.GetTestsInSpec);
            }
            catch
            {
                //TODO: Unload assembly context here when supported
                throw;
            }

            return new TestCollection(sourcesAsList, assemblyContext, tests);
        }
    }
}
