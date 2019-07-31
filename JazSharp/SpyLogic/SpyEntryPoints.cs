namespace JazSharp.SpyLogic
{
    internal static class SpyEntryPoints
    {
        internal static void Action0()
        {
            SpyExecutionHelper.HandleCall(0, null, new object[0]);
        }

        internal static void Action1<TP1>(TP1 p1)
        {
            SpyExecutionHelper.HandleCall(0, null, new object[] { p1 });
        }
    }
}
