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

        public static void Action3<TP1, TP2, TP3>(TP1 p1, TP2 p2, TP3 p3)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3 }, false);
        }

        public static void Action4<TP1, TP2, TP3, TP4>(TP1 p1, TP2 p2, TP3 p3, TP4 p4)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4 }, false);
        }

        public static void Action5<TP1, TP2, TP3, TP4, TP5>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5 }, false);
        }

        public static void Action6<TP1, TP2, TP3, TP4, TP5, TP6>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6 }, false);
        }

        public static void Action7<TP1, TP2, TP3, TP4, TP5, TP6, TP7>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, TP7 p7)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6, p7 }, false);
        }

        public static void Action8<TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, TP7 p7, TP8 p8)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6, p7, p8 }, false);
        }

        public static TR Func0<TR>()
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[0], true);
        }

        public static TR Func1<TR, TP1>(TP1 p1)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1 }, true);
        }

        public static TR Func2<TR, TP1, TP2>(TP1 p1, TP2 p2)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1, p2 }, true);
        }

        public static TR Func3<TR, TP1, TP2, TP3>(TP1 p1, TP2 p2, TP3 p3)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3 }, true);
        }

        public static TR Func4<TR, TP1, TP2, TP3, TP4>(TP1 p1, TP2 p2, TP3 p3, TP4 p4)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4 }, true);
        }

        public static TR Func5<TR, TP1, TP2, TP3, TP4, TP5>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5 }, true);
        }

        public static TR Func6<TR, TP1, TP2, TP3, TP4, TP5, TP6>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6 }, true);
        }

        public static TR Func7<TR, TP1, TP2, TP3, TP4, TP5, TP6, TP7>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, TP7 p7)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6, p7 }, true);
        }

        public static TR Func8<TR, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, TP7 p7, TP8 p8)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6, p7, p8 }, true);
        }
    }
}
