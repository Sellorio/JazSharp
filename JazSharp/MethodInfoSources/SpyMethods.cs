using JazSharp.SpyLogic;
using System.Reflection;

namespace JazSharp.MethodInfoSources
{
    internal class SpyMethods
    {
        public void InstanceAction0()
        {
            SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), this, new object[0]);
        }

        public void InstanceAction1(object p1)
        {
            SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), this, new[] { p1 });
        }

        public void InstanceAction2(object p1, object p2)
        {
            SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), this, new[] { p1, p2 });
        }

        public void InstanceAction3(object p1, object p2, object p3)
        {
            SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), this, new[] { p1, p2, p3 });
        }

        public void InstanceAction4(object p1, object p2, object p3, object p4)
        {
            SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), this, new[] { p1, p2, p3, p4 });
        }

        public void InstanceAction5(object p1, object p2, object p3, object p4, object p5)
        {
            SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), this, new[] { p1, p2, p3, p4, p5 });
        }

        public void InstanceAction6(object p1, object p2, object p3, object p4, object p5, object p6)
        {
            SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), this, new[] { p1, p2, p3, p4, p5, p6 });
        }

        public void InstanceAction7(object p1, object p2, object p3, object p4, object p5, object p6, object p7)
        {
            SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), this, new[] { p1, p2, p3, p4, p5, p6, p7 });
        }

        public void InstanceAction7(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8)
        {
            SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), this, new[] { p1, p2, p3, p4, p5, p6, p7, p8 });
        }

        public object InstanceFunc0()
            => SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), this, new object[0]);
        public object InstanceFunc1(object p1)
            => SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), this, new[] { p1 });
        public object InstanceFunc2(object p1, object p2)
            => SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), this, new[] { p1, p2 });
        public object InstanceFunc3(object p1, object p2, object p3)
            => SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), this, new[] { p1, p2, p3 });
        public object InstanceFunc4(object p1, object p2, object p3, object p4)
            => SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), this, new[] { p1, p2, p3, p4 });
        public object InstanceFunc5(object p1, object p2, object p3, object p4, object p5)
            => SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), this, new[] { p1, p2, p3, p4, p5 });
        public object InstanceFunc6(object p1, object p2, object p3, object p4, object p5, object p6)
            => SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), this, new[] { p1, p2, p3, p4, p5, p6 });
        public object InstanceFunc7(object p1, object p2, object p3, object p4, object p5, object p6, object p7)
            => SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), this, new[] { p1, p2, p3, p4, p5, p6, p7 });
        public object InstanceFunc8(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8)
            => SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), this, new[] { p1, p2, p3, p4, p5, p6, p7, p8 });

        public static void StaticAction0()
        {
            SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), null, new object[0]);
        }

        public static void StaticAction1(object p1)
        {
            SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), null, new[] { p1 });
        }

        public static void StaticAction2(object p1, object p2)
        {
            SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), null, new[] { p1, p2 });
        }

        public static void StaticAction3(object p1, object p2, object p3)
        {
            SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), null, new[] { p1, p2, p3 });
        }

        public static void StaticAction4(object p1, object p2, object p3, object p4)
        {
            SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), null, new[] { p1, p2, p3, p4 });
        }

        public static void StaticAction5(object p1, object p2, object p3, object p4, object p5)
        {
            SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), null, new[] { p1, p2, p3, p4, p5 });
        }

        public static void StaticAction6(object p1, object p2, object p3, object p4, object p5, object p6)
        {
            SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), null, new[] { p1, p2, p3, p4, p5, p6 });
        }

        public static void StaticAction7(object p1, object p2, object p3, object p4, object p5, object p6, object p7)
        {
            SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), null, new[] { p1, p2, p3, p4, p5, p6, p7 });
        }

        public static void StaticAction7(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8)
        {
            SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), null, new[] { p1, p2, p3, p4, p5, p6, p7, p8 });
        }

        public static object StaticFunc0()
            => SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), null, new object[0]);
        public static object StaticFunc1(object p1)
            => SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), null, new[] { p1 });
        public static object StaticFunc2(object p1, object p2)
            => SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), null, new[] { p1, p2 });
        public static object StaticFunc3(object p1, object p2, object p3)
            => SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), null, new[] { p1, p2, p3 });
        public static object StaticFunc4(object p1, object p2, object p3, object p4)
            => SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), null, new[] { p1, p2, p3, p4 });
        public static object StaticFunc5(object p1, object p2, object p3, object p4, object p5)
            => SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), null, new[] { p1, p2, p3, p4, p5 });
        public static object StaticFunc6(object p1, object p2, object p3, object p4, object p5, object p6)
            => SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), null, new[] { p1, p2, p3, p4, p5, p6 });
        public static object StaticFunc7(object p1, object p2, object p3, object p4, object p5, object p6, object p7)
            => SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), null, new[] { p1, p2, p3, p4, p5, p6, p7 });
        public static object StaticFunc8(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8)
            => SpyExecutionHelper.HandleCall(MethodBase.GetCurrentMethod(), null, new[] { p1, p2, p3, p4, p5, p6, p7, p8 });

        internal static MethodInfo GetSpyForMethod(MethodInfo methodInfo)
        {
            MethodInfo result;

            if (methodInfo.IsStatic)
            {
                if (methodInfo.ReturnType != typeof(void))
                {
                    result = typeof(SpyMethods).GetMethod("StaticFunc" + methodInfo.GetParameters().Length);
                }
                else
                {
                    result = typeof(SpyMethods).GetMethod("StaticAction" + methodInfo.GetParameters().Length);
                }
            }
            else
            {
                if (methodInfo.ReturnType != typeof(void))
                {
                    result = typeof(SpyMethods).GetMethod("InstanceFunc" + methodInfo.GetParameters().Length);
                }
                else
                {
                    result = typeof(SpyMethods).GetMethod("InstanceAction" + methodInfo.GetParameters().Length);
                }
            }

            return result;
        }
    }
}
