using JazSharp.Spies;
using System;

namespace JazSharp.SpyLogic
{
    internal static class DynamicSpyHelper
    {
        internal static Action CreateSpy(out Spy spy)
        {
            Spy newSpy = null;
            Action result = null;

            result = () =>
            {
                SpyExecutionHelper.InnerHandleCall(newSpy, result.Method, result.Target, new object[0]);
            };

            return MakeSpy(result, out newSpy, out spy);
        }

        internal static Action<TParam1> CreateSpy<TParam1>(out Spy spy)
        {
            Spy newSpy = null;
            Action<TParam1> result = null;

            result = p1 =>
            {
                SpyExecutionHelper.InnerHandleCall(newSpy, result.Method, result.Target, new object[] { p1 });
            };

            return MakeSpy(result, out newSpy, out spy);
        }

        internal static Action<TParam1, TParam2> CreateSpy<TParam1, TParam2>(out Spy spy)
        {
            Spy newSpy = null;
            Action<TParam1, TParam2> result = null;

            result = (p1, p2) =>
            {
                SpyExecutionHelper.InnerHandleCall(newSpy, result.Method, result.Target, new object[] { p1, p2 });
            };

            return MakeSpy(result, out newSpy, out spy);
        }

        internal static Action<TParam1, TParam2, TParam3> CreateSpy<TParam1, TParam2, TParam3>(out Spy spy)
        {
            Spy newSpy = null;
            Action<TParam1, TParam2, TParam3> result = null;

            result = (p1, p2, p3) =>
            {
                SpyExecutionHelper.InnerHandleCall(newSpy, result.Method, result.Target, new object[] { p1, p2, p3 });
            };

            return MakeSpy(result, out newSpy, out spy);
        }

        internal static Action<TParam1, TParam2, TParam3, TParam4> CreateSpy<TParam1, TParam2, TParam3, TParam4>(out Spy spy)
        {
            Spy newSpy = null;
            Action<TParam1, TParam2, TParam3, TParam4> result = null;

            result = (p1, p2, p3, p4) =>
            {
                SpyExecutionHelper.InnerHandleCall(newSpy, result.Method, result.Target, new object[] { p1, p2, p3, p4 });
            };

            return MakeSpy(result, out newSpy, out spy);
        }

        internal static Action<TParam1, TParam2, TParam3, TParam4, TParam5> CreateSpy<TParam1, TParam2, TParam3, TParam4, TParam5>(out Spy spy)
        {
            Spy newSpy = null;
            Action<TParam1, TParam2, TParam3, TParam4, TParam5> result = null;

            result = (p1, p2, p3, p4, p5) =>
            {
                SpyExecutionHelper.InnerHandleCall(newSpy, result.Method, result.Target, new object[] { p1, p2, p3, p4, p5 });
            };
            
            return MakeSpy(result, out newSpy, out spy);
        }

        internal static Func<TResult> CreateSpyFunc<TResult>(out Spy spy)
        {
            Spy newSpy = null;
            Func<TResult> result = null;

            result = () =>
            {
                return (TResult)SpyExecutionHelper.InnerHandleCall(newSpy, result.Method, result.Target, new object[0]);
            };

            return MakeSpy(result, out newSpy, out spy);
        }

        internal static Func<TParam1, TResult> CreateSpyFunc<TParam1, TResult>(out Spy spy)
        {
            Spy newSpy = null;
            Func<TParam1, TResult> result = null;

            result = p1 =>
            {
                return (TResult)SpyExecutionHelper.InnerHandleCall(newSpy, result.Method, result.Target, new object[] { p1 });
            };

            return MakeSpy(result, out newSpy, out spy);
        }

        internal static Func<TParam1, TParam2, TResult> CreateSpyFunc<TParam1, TParam2, TResult>(out Spy spy)
        {
            Spy newSpy = null;
            Func<TParam1, TParam2, TResult> result = null;

            result = (p1, p2) =>
            {
                return (TResult)SpyExecutionHelper.InnerHandleCall(newSpy, result.Method, result.Target, new object[] { p1, p2 });
            };

            return MakeSpy(result, out newSpy, out spy);
        }

        internal static Func<TParam1, TParam2, TParam3, TResult> CreateSpyFunc<TParam1, TParam2, TParam3, TResult>(out Spy spy)
        {
            Spy newSpy = null;
            Func<TParam1, TParam2, TParam3, TResult> result = null;

            result = (p1, p2, p3) =>
            {
                return (TResult)SpyExecutionHelper.InnerHandleCall(newSpy, result.Method, result.Target, new object[] { p1, p2, p3 });
            };

            return MakeSpy(result, out newSpy, out spy);
        }

        internal static Func<TParam1, TParam2, TParam3, TParam4, TResult> CreateSpyFunc<TParam1, TParam2, TParam3, TParam4, TResult>(out Spy spy)
        {
            Spy newSpy = null;
            Func<TParam1, TParam2, TParam3, TParam4, TResult> result = null;

            result = (p1, p2, p3, p4) =>
            {
                return (TResult)SpyExecutionHelper.InnerHandleCall(newSpy, result.Method, result.Target, new object[] { p1, p2, p3, p4 });
            };

            return MakeSpy(result, out newSpy, out spy);
        }

        internal static Func<TParam1, TParam2, TParam3, TParam4, TParam5, TResult> CreateSpyFunc<TParam1, TParam2, TParam3, TParam4, TParam5, TResult>(out Spy spy)
        {
            Spy newSpy = null;
            Func<TParam1, TParam2, TParam3, TParam4, TParam5, TResult> result = null;

            result = (p1, p2, p3, p4, p5) =>
            {
                return (TResult)SpyExecutionHelper.InnerHandleCall(newSpy, result.Method, result.Target, new object[] { p1, p2, p3, p4, p5 });
            };

            return MakeSpy(result, out newSpy, out spy);
        }

        private static TDelegate MakeSpy<TDelegate>(TDelegate @delegate, out Spy spyVariable, out Spy spyOutParameter)
            where TDelegate : Delegate
        {
            spyVariable = Spy.Create(new[] { @delegate.Method }, Guid.NewGuid());
            spyOutParameter = spyVariable;

            return @delegate;
        }
    }
}
