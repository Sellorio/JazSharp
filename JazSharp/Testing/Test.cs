using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace JazSharp.Testing
{
    /// <summary>
    /// Describes a test that has been discovered.
    /// </summary>
    public class Test
    {
        internal int TestMetadataToken { get; }
        internal string AssemblyName { get; }

        /// <summary>
        /// The class that defined this test.
        /// </summary>
        public Type TestClass { get; }

        /// <summary>
        /// Whether or not the test (or any of it's ancestor Describes) has made it a focus. If any tests are
        /// focused, only focused tests are executed. If a test is excluded, it cannot be focused. Tests can
        /// be focused by using <see cref="Spec.fIt(string, Action, string, int)"/> or <see cref="Spec.fDescribe(string, Action)"/>.
        /// </summary>
        public bool IsFocused { get; }

        /// <summary>
        /// Whether or not the test (or any of it's ancestor Describes) has been flagged for exclusion. Excluded
        /// tests are never executed and will be reported as Skipped in some test runners. An excluded test will
        /// never be focused. Tests can be excluded by using <see cref="Spec.xIt(string, Action, string, int)"/> and
        /// <see cref="Spec.xDescribe(string, Action)"/>.
        /// </summary>
        public bool IsExcluded { get; }

        /// <summary>
        /// The path to the test. Each element in the path is comprised of the value passed into a call to
        /// <see cref="Spec.Describe(string, Action)"/> and similar methods.
        /// </summary>
        public ImmutableArray<string> Path { get; }
        
        /// <summary>
        /// The description of the test. This is the value passed in to <see cref="Spec.It(string, Action, string, int)"/> and
        /// similar methods.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The full name of the test. This is a concatenation of the <see cref="Path"/> values and the
        /// <see cref="Description"/>.
        /// </summary>
        public string FullName { get; }

        /// <summary>
        /// The file name of the assembly in which this test resides.
        /// </summary>
        public string AssemblyFilename { get; }

        /// <summary>
        /// The filename of the source file where the <see cref="Spec.It(string, Action, string, int)"/> call was made.
        /// </summary>
        public string SourceFilename { get; }

        /// <summary>
        /// The line number within <see cref="SourceFilename"/> where the <see cref="Spec.It(string, Action, string, int)"/>
        /// call is made.
        /// </summary>
        public int LineNumber { get; }

        internal Test(
            Type testClass,
            IEnumerable<string> path,
            string description,
            Delegate execution,
            bool isFocused,
            bool isExcluded,
            string sourceFilename,
            int lineNumber)
        {
            TestClass = testClass;
            TestMetadataToken = execution.Method.MetadataToken;
            AssemblyName = execution.Method.DeclaringType.Assembly.FullName;
            AssemblyFilename = execution.Method.DeclaringType.Assembly.Location;
            Path = ImmutableArray.CreateRange(path);
            Description = description;
            FullName = string.Join(" ", path) + " " + description;
            IsFocused = isFocused;
            IsExcluded = isExcluded;
            SourceFilename = sourceFilename;
            LineNumber = lineNumber;
        }

        internal RunnableTest Prepare(Assembly executionReadyAssembly)
        {
            var module = executionReadyAssembly.Modules.First();
            var execution = SpecHelper.GetPreparedTestExecution(module.ResolveType(TestClass.MetadataToken), TestMetadataToken);

            return new RunnableTest(TestClass, Path, Description, execution, IsFocused, IsExcluded, SourceFilename, LineNumber);
        }
    }
}
