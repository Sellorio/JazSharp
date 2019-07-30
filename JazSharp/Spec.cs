using JazSharp.TestSetup;
using System;
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

        public static void It(string testDescription, Action test)
        {
            SpecHelper.RegisterTest(testDescription, test, false, false);
        }

        public static void xIt(string testDescription, Action test)
        {
            SpecHelper.RegisterTest(testDescription, test, false, true);
        }

        public static void fIt(string testDescription, Action test)
        {
            SpecHelper.RegisterTest(testDescription, test, true, false);
        }

        public static void It(string testDescription, Func<Task> test)
        {
            SpecHelper.RegisterTest(testDescription, test, false, false);
        }

        public static void xIt(string testDescription, Func<Task> test)
        {
            SpecHelper.RegisterTest(testDescription, test, false, true);
        }

        public static void fIt(string testDescription, Func<Task> test)
        {
            SpecHelper.RegisterTest(testDescription, test, true, false);
        }
    }
}
#pragma warning restore IDE1006 // Naming Styles
