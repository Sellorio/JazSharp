using JazSharp.Expectations;
using JazSharp.Spies;
using JazSharp.Testing;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

#pragma warning disable IDE1006 // Naming Styles
namespace JazSharp
{
    public abstract class Spec
    {
        public static void Describe<TType>(Action definition)
        {
            SpecHelper.PushDescribe(typeof(TType).Name, false, false);
            definition();
            SpecHelper.PopDescribe();
        }

        public static void Describe(string description, Action definition)
        {
            SpecHelper.PushDescribe(description, false, false);
            definition();
            SpecHelper.PopDescribe();
        }

        public static void xDescribe<TType>(Action definition)
        {
            SpecHelper.PushDescribe(typeof(TType).Name, false, true);
            definition();
            SpecHelper.PopDescribe();
        }

        public static void xDescribe(string childDescription, Action definition)
        {
            SpecHelper.PushDescribe(childDescription, false, true);
            definition();
            SpecHelper.PopDescribe();
        }

        public static void fDescribe<TType>(Action definition)
        {
            SpecHelper.PushDescribe(typeof(TType).Name, true, false);
            definition();
            SpecHelper.PopDescribe();
        }

        public static void fDescribe(string childDescription, Action definition)
        {
            SpecHelper.PushDescribe(childDescription, true, false);
            definition();
            SpecHelper.PopDescribe();
        }

        public static void It(string testDescription, Action test, [CallerFilePath] string sourceFile = default, [CallerLineNumber] int lineNumber = default)
        {
            SpecHelper.RegisterTest(testDescription, test, false, false, sourceFile, lineNumber);
        }

        public static void xIt(string testDescription, Action test, [CallerFilePath] string sourceFile = default, [CallerLineNumber] int lineNumber = default)
        {
            SpecHelper.RegisterTest(testDescription, test, false, true, sourceFile, lineNumber);
        }

        public static void fIt(string testDescription, Action test, [CallerFilePath] string sourceFile = default, [CallerLineNumber] int lineNumber = default)
        {
            SpecHelper.RegisterTest(testDescription, test, true, false, sourceFile, lineNumber);
        }

        public static void It(string testDescription, Func<Task> test, [CallerFilePath] string sourceFile = default, [CallerLineNumber] int lineNumber = default)
        {
            SpecHelper.RegisterTest(testDescription, test, false, false, sourceFile, lineNumber);
        }

        public static void xIt(string testDescription, Func<Task> test, [CallerFilePath] string sourceFile = default, [CallerLineNumber] int lineNumber = default)
        {
            SpecHelper.RegisterTest(testDescription, test, false, true, sourceFile, lineNumber);
        }

        public static void fIt(string testDescription, Func<Task> test, [CallerFilePath] string sourceFile = default, [CallerLineNumber] int lineNumber = default)
        {
            SpecHelper.RegisterTest(testDescription, test, true, false, sourceFile, lineNumber);
        }

        public static SpyExpect Expect(Spy spy)
        {
            return new SpyExpect(spy);
        }

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

        public static ValueExpect<TValue> Expect<TValue>(TValue value)
        {
            return new ValueExpect<TValue>(value);
        }

        public static void Fail(string message = null)
        {
            throw new JazExpectationException(message ?? "The test was explicitly failed.");
        }
    }
}
#pragma warning restore IDE1006 // Naming Styles
