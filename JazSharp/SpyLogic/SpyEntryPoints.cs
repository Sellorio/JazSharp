using System.Reflection;

namespace JazSharp.SpyLogic
{
    /// <summary>
    /// Do not use this class. This class is only present to enable spies to work.
    /// </summary>
    public static class SpyEntryPoints
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static void Action0(MethodBase originalMethod)
        {
            SpyExecutionHelper.HandleCall(new object[0], originalMethod);
        }

        public static void Action1<TP1>(TP1 p1, MethodBase originalMethod)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1 }, originalMethod);
        }

        public static void Action2<TP1, TP2>(TP1 p1, TP2 p2, MethodBase originalMethod)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1, p2 }, originalMethod);
        }

        public static void Action3<TP1, TP2, TP3>(TP1 p1, TP2 p2, TP3 p3, MethodBase originalMethod)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3 }, originalMethod);
        }

        public static void Action4<TP1, TP2, TP3, TP4>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, MethodBase originalMethod)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4 }, originalMethod);
        }

        public static void Action5<TP1, TP2, TP3, TP4, TP5>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, MethodBase originalMethod)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5 }, originalMethod);
        }

        public static void Action6<TP1, TP2, TP3, TP4, TP5, TP6>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, MethodBase originalMethod)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6 }, originalMethod);
        }

        public static void Action7<TP1, TP2, TP3, TP4, TP5, TP6, TP7>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, TP7 p7, MethodBase originalMethod)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6, p7 }, originalMethod);
        }

        public static void Action8<TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, TP7 p7, TP8 p8, MethodBase originalMethod)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6, p7, p8 }, originalMethod);
        }

        public static void Action8<TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, TP7 p7, TP8 p8, TP9 p9, MethodBase originalMethod)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6, p7, p8, p9 }, originalMethod);
        }

        public static void Action8<TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, TP7 p7, TP8 p8, TP9 p9, TP10 p10, MethodBase originalMethod)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10 }, originalMethod);
        }

        public static void Action8<TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, TP7 p7, TP8 p8, TP9 p9, TP10 p10, TP11 p11, MethodBase originalMethod)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11 }, originalMethod);
        }

        public static void Action8<TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, TP7 p7, TP8 p8, TP9 p9, TP10 p10, TP11 p11, TP12 p12, MethodBase originalMethod)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12 }, originalMethod);
        }

        public static void Action8<TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, TP7 p7, TP8 p8, TP9 p9, TP10 p10, TP11 p11, TP12 p12, TP13 p13, MethodBase originalMethod)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13 }, originalMethod);
        }

        public static void Action8<TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, TP7 p7, TP8 p8, TP9 p9, TP10 p10, TP11 p11, TP12 p12, TP13 p13, TP14 p14, MethodBase originalMethod)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14 }, originalMethod);
        }

        public static void Action8<TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TP15>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, TP7 p7, TP8 p8, TP9 p9, TP10 p10, TP11 p11, TP12 p12, TP13 p13, TP14 p14, TP15 p15, MethodBase originalMethod)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15 }, originalMethod);
        }

        public static void Action8<TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TP15, TP16>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, TP7 p7, TP8 p8, TP9 p9, TP10 p10, TP11 p11, TP12 p12, TP13 p13, TP14 p14, TP15 p15, TP16 p16, MethodBase originalMethod)
        {
            SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16 }, originalMethod);
        }

        public static TR Func0<TR>(MethodBase originalMethod)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[0], originalMethod);
        }

        public static TR Func1<TR, TP1>(TP1 p1, MethodBase originalMethod)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1 }, originalMethod);
        }

        public static TR Func2<TR, TP1, TP2>(TP1 p1, TP2 p2, MethodBase originalMethod)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1, p2 }, originalMethod);
        }

        public static TR Func3<TR, TP1, TP2, TP3>(TP1 p1, TP2 p2, TP3 p3, MethodBase originalMethod)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3 }, originalMethod);
        }

        public static TR Func4<TR, TP1, TP2, TP3, TP4>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, MethodBase originalMethod)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4 }, originalMethod);
        }

        public static TR Func5<TR, TP1, TP2, TP3, TP4, TP5>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, MethodBase originalMethod)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5 }, originalMethod);
        }

        public static TR Func6<TR, TP1, TP2, TP3, TP4, TP5, TP6>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, MethodBase originalMethod)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6 }, originalMethod);
        }

        public static TR Func7<TR, TP1, TP2, TP3, TP4, TP5, TP6, TP7>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, TP7 p7, MethodBase originalMethod)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6, p7 }, originalMethod);
        }

        public static TR Func8<TR, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, TP7 p7, TP8 p8, MethodBase originalMethod)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6, p7, p8 }, originalMethod);
        }

        public static TR Func8<TR, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, TP7 p7, TP8 p8, TP9 p9, MethodBase originalMethod)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6, p7, p8, p9 }, originalMethod);
        }

        public static TR Func8<TR, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, TP7 p7, TP8 p8, TP9 p9, TP10 p10, MethodBase originalMethod)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10 }, originalMethod);
        }

        public static TR Func8<TR, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, TP7 p7, TP8 p8, TP9 p9, TP10 p10, TP11 p11, MethodBase originalMethod)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11 }, originalMethod);
        }

        public static TR Func8<TR, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, TP7 p7, TP8 p8, TP9 p9, TP10 p10, TP11 p11, TP12 p12, MethodBase originalMethod)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12 }, originalMethod);
        }

        public static TR Func8<TR, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, TP7 p7, TP8 p8, TP9 p9, TP10 p10, TP11 p11, TP12 p12, TP13 p13, MethodBase originalMethod)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13 }, originalMethod);
        }

        public static TR Func8<TR, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, TP7 p7, TP8 p8, TP9 p9, TP10 p10, TP11 p11, TP12 p12, TP13 p13, TP14 p14, MethodBase originalMethod)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14 }, originalMethod);
        }

        public static TR Func8<TR, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TP15>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, TP7 p7, TP8 p8, TP9 p9, TP10 p10, TP11 p11, TP12 p12, TP13 p13, TP14 p14, TP15 p15, MethodBase originalMethod)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15 }, originalMethod);
        }

        public static TR Func8<TR, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TP15, TP16>(TP1 p1, TP2 p2, TP3 p3, TP4 p4, TP5 p5, TP6 p6, TP7 p7, TP8 p8, TP9 p9, TP10 p10, TP11 p11, TP12 p12, TP13 p13, TP14 p14, TP15 p15, TP16 p16, MethodBase originalMethod)
        {
            return (TR)SpyExecutionHelper.HandleCall(new object[] { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16 }, originalMethod);
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
