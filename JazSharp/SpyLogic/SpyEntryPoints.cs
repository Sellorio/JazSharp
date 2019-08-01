using System;

namespace JazSharp.SpyLogic
{
    public static class SpyEntryPoints
    {
        public static void Action0()
        {
            SpyExecutionHelper.HandleCall(new object[0], false);
        }

        public static void Action1<TP1>(TP1 p1)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1 }, false);
        }

        public static void Action2<TP1, TP2>(TP1 p1, TP2 p2)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1, p2 }, false);
        }

        public static TR Func0<TR>()
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[0], true);
        }

        public static TR Func1<TR, TP1>(TP1 p1)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1 }, true);
        }
    }
}
