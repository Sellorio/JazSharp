using System;

namespace JazSharp.SpyLogic
{
    public static class SpyEntryPoints
    {
        public static void Action0()
        {
            SpyExecutionHelper.HandleCall(0, null, new object[0]);
        }

        public static void Action1<TP1>(TP1 p1)
        {
            SpyExecutionHelper.HandleCall(0, null, new object[] { p1 });
        }

        public static void Action2<TP1, TP2>(TP1 p1, TP2 p2)
        {
            SpyExecutionHelper.HandleCall(0, null, new object[] { p1, p2 });
        }

        public static TR Func0<TR>()
        {
            return (TR)SpyExecutionHelper.HandleCall(0, null, new object[0]);
        }

        public static TR Func1<TR, TP1>(TP1 p1)
        {
            return (TR)SpyExecutionHelper.HandleCall(0, null, new object[] { p1 });
        }

        //public static void Action2(object instance, Action<string> p1)
        //{
        //}

        //public static string Func1(object instance)
        //{
        //    return "blah";
        //}
    }
}
