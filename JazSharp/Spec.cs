﻿using JazSharp.Expectations;
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
        }

        public static void Describe(string childDescription, Action definition)
        {
            SpecHelper.PushDescribe(childDescription, false, false);
            definition();
        }

        public static void xDescribe<TType>(Action definition)
        {
            SpecHelper.PushDescribe(typeof(TType).Name, false, true);
            definition();
        }

        public static void xDescribe(string childDescription, Action definition)
        {
            SpecHelper.PushDescribe(childDescription, false, true);
            definition();
        }

        public static void fDescribe<TType>(Action definition)
        {
            SpecHelper.PushDescribe(typeof(TType).Name, true, false);
            definition();
        }

        public static void fDescribe(string childDescription, Action definition)
        {
            SpecHelper.PushDescribe(childDescription, true, false);
            definition();
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

        public static ValueExpect Expect(object value)
        {
            return new ValueExpect(value);
        }
    }
}
#pragma warning restore IDE1006 // Naming Styles