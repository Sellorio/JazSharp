using JazSharp.Expectations;
using JazSharp.Spies;
using JazSharp.Testing;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

#pragma warning disable IDE1006 // Naming Styles
namespace JazSharp
{
    /// <summary>
    /// The base class that is required for all test classes.
    /// </summary>
    public abstract class Spec
    {
        /// <summary>
        /// Describes the type that included tests will be testing. It is strongly recommended
        /// to have this as the root describe for any test class.
        /// </summary>
        /// <typeparam name="TType">The type being tested.</typeparam>
        /// <param name="definition">The definition of the describe.</param>
        public static void Describe<TType>(Action definition)
        {
            SpecHelper.PushDescribe(typeof(TType).Name, false, false);
            definition();
            SpecHelper.PopDescribe();
        }

        /// <summary>
        /// Describes a context for a test. This should usually be either a member name or
        /// a scenario whose tests are grouped together.
        /// </summary>
        /// <param name="description">The name of the member, the scenario description or similar.</param>
        /// <param name="definition">The definition of the describe.</param>
        public static void Describe(string description, Action definition)
        {
            SpecHelper.PushDescribe(description, false, false);
            definition();
            SpecHelper.PopDescribe();
        }

        /// <summary>
        /// Excludes the tests contained in this describe from any test runs.
        /// </summary>
        /// <param name="definition">The definition of the describe.</param>
        public static void xDescribe<TType>(Action definition)
        {
            SpecHelper.PushDescribe(typeof(TType).Name, false, true);
            definition();
            SpecHelper.PopDescribe();
        }

        /// <summary>
        /// Excludes the tests contained in this describe from any test runs.
        /// </summary>
        /// <param name="childDescription">The description of the describe.</param>
        /// <param name="definition">The definition of the describe.</param>
        public static void xDescribe(string childDescription, Action definition)
        {
            SpecHelper.PushDescribe(childDescription, false, true);
            definition();
            SpecHelper.PopDescribe();
        }

        /// <summary>
        /// Focuses the tests contained in this describe. All non-focused tests will
        /// be excluded from the test run.
        /// </summary>
        /// <param name="definition">The definition of the describe.</param>
        public static void fDescribe<TType>(Action definition)
        {
            SpecHelper.PushDescribe(typeof(TType).Name, true, false);
            definition();
            SpecHelper.PopDescribe();
        }

        /// <summary>
        /// Focuses the tests contained in this describe. All non-focused tests will
        /// be excluded from the test run.
        /// </summary>
        /// <param name="childDescription">The description of the describe.</param>
        /// <param name="definition">The definition of the describe.</param>
        public static void fDescribe(string childDescription, Action definition)
        {
            SpecHelper.PushDescribe(childDescription, true, false);
            definition();
            SpecHelper.PopDescribe();
        }

        /// <summary>
        /// Defines a test. All tests must be contained in at least 1 level of Describes.
        /// </summary>
        /// <param name="testDescription">The description of the test. The description should start with "should".</param>
        /// <param name="test">The implementation of the test.</param>
        /// <param name="sourceFile">Do not manually specify this parameter.</param>
        /// <param name="lineNumber">Do not manually specify this parameter.</param>
        public static void It(string testDescription, Action test, [CallerFilePath] string sourceFile = default, [CallerLineNumber] int lineNumber = default)
        {
            SpecHelper.RegisterTest(testDescription, test, false, false, sourceFile, lineNumber);
        }

        /// <summary>
        /// Excludes the test from all test runs.
        /// </summary>
        /// <param name="testDescription">The description of the test. The description should start with "should".</param>
        /// <param name="test">The implementation of the test.</param>
        /// <param name="sourceFile">Do not manually specify this parameter.</param>
        /// <param name="lineNumber">Do not manually specify this parameter.</param>
        public static void xIt(string testDescription, Action test, [CallerFilePath] string sourceFile = default, [CallerLineNumber] int lineNumber = default)
        {
            SpecHelper.RegisterTest(testDescription, test, false, true, sourceFile, lineNumber);
        }

        /// <summary>
        /// Focuses the test. Any non-focused tests will be exlcuded from the test runs.
        /// </summary>
        /// <param name="testDescription">The description of the test. The description should start with "should".</param>
        /// <param name="test">The implementation of the test.</param>
        /// <param name="sourceFile">Do not manually specify this parameter.</param>
        /// <param name="lineNumber">Do not manually specify this parameter.</param>
        public static void fIt(string testDescription, Action test, [CallerFilePath] string sourceFile = default, [CallerLineNumber] int lineNumber = default)
        {
            SpecHelper.RegisterTest(testDescription, test, true, false, sourceFile, lineNumber);
        }

        /// <summary>
        /// Defines a test. All tests must be contained in at least 1 level of Describes.
        /// </summary>
        /// <param name="testDescription">The description of the test. The description should start with "should".</param>
        /// <param name="test">The implementation of the test.</param>
        /// <param name="sourceFile">Do not manually specify this parameter.</param>
        /// <param name="lineNumber">Do not manually specify this parameter.</param>
        public static void It(string testDescription, Func<Task> test, [CallerFilePath] string sourceFile = default, [CallerLineNumber] int lineNumber = default)
        {
            SpecHelper.RegisterTest(testDescription, test, false, false, sourceFile, lineNumber);
        }

        /// <summary>
        /// Excludes the test from all test runs.
        /// </summary>
        /// <param name="testDescription">The description of the test. The description should start with "should".</param>
        /// <param name="test">The implementation of the test.</param>
        /// <param name="sourceFile">Do not manually specify this parameter.</param>
        /// <param name="lineNumber">Do not manually specify this parameter.</param>
        public static void xIt(string testDescription, Func<Task> test, [CallerFilePath] string sourceFile = default, [CallerLineNumber] int lineNumber = default)
        {
            SpecHelper.RegisterTest(testDescription, test, false, true, sourceFile, lineNumber);
        }

        /// <summary>
        /// Focuses the test. Any non-focused tests will be exlcuded from the test runs.
        /// </summary>
        /// <param name="testDescription">The description of the test. The description should start with "should".</param>
        /// <param name="test">The implementation of the test.</param>
        /// <param name="sourceFile">Do not manually specify this parameter.</param>
        /// <param name="lineNumber">Do not manually specify this parameter.</param>
        public static void fIt(string testDescription, Func<Task> test, [CallerFilePath] string sourceFile = default, [CallerLineNumber] int lineNumber = default)
        {
            SpecHelper.RegisterTest(testDescription, test, true, false, sourceFile, lineNumber);
        }

        /// <summary>
        /// Defines logic that will execute before each test. This is used to initialise
        /// variables, spies and static values that are shared among multiple tests. The
        /// logic contained will only execute for tests in the current Describe context
        /// or any child Describe contexts. Before eaches can only be defined inside a
        /// Describe.
        /// </summary>
        /// <param name="beforeEach">The logic to be executed.</param>
        public static void BeforeEach(Action beforeEach)
        {
            SpecHelper.AddBeforeEach(beforeEach);
        }

        /// <summary>
        /// Defines logic that will execute before each test. This is used to initialise
        /// variables, spies and static values that are shared among multiple tests. The
        /// logic contained will only execute for tests in the current Describe context
        /// or any child Describe contexts. Before eaches can only be defined inside a
        /// Describe.
        /// </summary>
        /// <param name="beforeEach">The logic to be executed.</param>
        public static void BeforeEach(Func<Task> beforeEach)
        {
            SpecHelper.AddBeforeEach(beforeEach);
        }

        /// <summary>
        /// Defines logic that will execute after each test. This can be used to clean up
        /// after a test or check some common expectations. The logic contained will only
        /// execute for tests in the current Describe context or any child Describe contexts.
        /// Before eaches can only be defined inside a Describe.
        /// </summary>
        /// <param name="afterEach">The logic to be executed.</param>
        public static void AfterEach(Action afterEach)
        {
            SpecHelper.AddAfterEach(afterEach);
        }

        /// <summary>
        /// Defines logic that will execute after each test. This can be used to clean up
        /// after a test or check some common expectations. The logic contained will only
        /// execute for tests in the current Describe context or any child Describe contexts.
        /// Before eaches can only be defined inside a Describe.
        /// </summary>
        /// <param name="afterEach">The logic to be executed.</param>
        public static void AfterEach(Func<Task> afterEach)
        {
            SpecHelper.AddAfterEach(afterEach);
        }

        /// <summary>
        /// Starts defining an expectation on a spy.
        /// </summary>
        /// <param name="spy">The spy being tested.</param>
        /// <returns>The object used to specify a specific expectation.</returns>
        public static SpyExpect Expect(Spy spy)
        {
            return new SpyExpect(spy);
        }

        /// <summary>
        /// Starts defining an expectation on a spy.
        /// </summary>
        /// <param name="spy">The spy being tested.</param>
        /// <returns>The object used to specify a specific expectation.</returns>
        public static SpyExpect Expect(SpyWithBehaviour spy)
        {
            return new SpyExpect(((ISpy)spy).Spy);
        }

        /// <summary>
        /// Starts defining an expectation on a spy.
        /// </summary>
        /// <param name="spy">The spy being tested.</param>
        /// <returns>The object used to specify a specific expectation.</returns>
        public static SpyExpect Expect(SpyWithQuantifiedBehaviour spy)
        {
            return new SpyExpect(((ISpy)spy).Spy);
        }

        /// <summary>
        /// Starts defining an expectation on a method call. This will catch any exceptions
        /// and allow you to specify which exception is expected.
        /// </summary>
        /// <param name="call">The method to call.</param>
        /// <returns>The object used to specify a specific expectation.</returns>
        public static CallExpect Expect(Action call)
        {
            Exception exception = null;

            try
            {
                call();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            return new CallExpect(exception);
        }

        /// <summary>
        /// Starts defining an expectation on a value.
        /// </summary>
        /// <param name="value">The value being tested.</param>
        /// <returns>The object used to specify a specific expectation.</returns>
        public static ValueExpect<TValue> Expect<TValue>(TValue value)
        {
            return new ValueExpect<TValue>(value);
        }

        /// <summary>
        /// Explicitly fails the test, optionally specifing the message to use.
        /// </summary>
        /// <param name="message">The message to use in the test failure.</param>
        public static void Fail(string message = null)
        {
            throw new JazExpectationException(message ?? "The test was explicitly failed.", 1);
        }
    }
}
#pragma warning restore IDE1006 // Naming Styles
