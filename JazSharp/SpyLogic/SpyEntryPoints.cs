using System;

namespace JazSharp.SpyLogic
{
    public static class SpyEntryPoints
    {
        public static void Action0(string serializedMethodCallInfo)
        {
            SpyExecutionHelper.HandleCall(new object[0], serializedMethodCallInfo);
        }

        public static void Action1<TP1>(TP1 p1, string serializedMethodCallInfo)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1 }, serializedMethodCallInfo);
        }

        public static void Action2<TP1, TP2>(TP1 p1, TP2 p2, string serializedMethodCallInfo)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1, p2 }, serializedMethodCallInfo);
        }

        public static void Action3<TP1, TP2, TP3>(TP1 p1, TP2 p2, TP3 p3, string serializedMethodCallInfo)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3 }, serializedMethodCallInfo);
        }

        public static void Action4<TP1, TP2, TP3, TP4>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, string serializedMethodCallInfo)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4 }, serializedMethodCallInfo);
        }

        public static void Action5<TP1, TP2, TP3, TP4, TP5>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, string serializedMethodCallInfo)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5 }, serializedMethodCallInfo);
        }

        public static void Action6<TP1, TP2, TP3, TP4, TP5, TP6>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, string serializedMethodCallInfo)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6 }, serializedMethodCallInfo);
        }

        public static void Action7<TP1, TP2, TP3, TP4, TP5, TP6, TP7>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, TP7 p7, string serializedMethodCallInfo)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6, p7 }, serializedMethodCallInfo);
        }

        public static void Action8<TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, TP7 p7, TP8 p8, string serializedMethodCallInfo)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6, p7, p8 }, serializedMethodCallInfo);
        }

        public static TR Func0<TR>(string serializedMethodCallInfo)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[0], serializedMethodCallInfo);
        }

        public static TR Func1<TR, TP1>(TP1 p1, string serializedMethodCallInfo)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1 }, serializedMethodCallInfo);
        }

        public static TR Func2<TR, TP1, TP2>(TP1 p1, TP2 p2, string serializedMethodCallInfo)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1, p2 }, serializedMethodCallInfo);
        }

        public static TR Func3<TR, TP1, TP2, TP3>(TP1 p1, TP2 p2, TP3 p3, string serializedMethodCallInfo)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3 }, serializedMethodCallInfo);
        }

        public static TR Func4<TR, TP1, TP2, TP3, TP4>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, string serializedMethodCallInfo)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4 }, serializedMethodCallInfo);
        }

        public static TR Func5<TR, TP1, TP2, TP3, TP4, TP5>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, string serializedMethodCallInfo)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5 }, serializedMethodCallInfo);
        }

        public static TR Func6<TR, TP1, TP2, TP3, TP4, TP5, TP6>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, string serializedMethodCallInfo)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6 }, serializedMethodCallInfo);
        }

        public static TR Func7<TR, TP1, TP2, TP3, TP4, TP5, TP6, TP7>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, TP7 p7, string serializedMethodCallInfo)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6, p7 }, serializedMethodCallInfo);
        }

        public static TR Func8<TR, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, TP7 p7, TP8 p8, string serializedMethodCallInfo)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6, p7, p8 }, serializedMethodCallInfo);
        }
    }
}
