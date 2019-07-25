using System.Runtime.CompilerServices;
using Xunit;

namespace JazSharp.Tests.Spies
{
    public class InstanceMethodTests
    {
        [Fact]
        public void ParameterlessAction()
        {
            var testInstance = new TestParameterlessAction();
            testInstance.Action();
            Assert.Equal(1, testInstance.Feedback);

            using (Jaz.SpyOn(testInstance, nameof(TestParameterlessAction.Action)))
            {
                testInstance.Action();
            }

            Assert.Equal(1, testInstance.Feedback);
            testInstance.Action();
            Assert.Equal(2, testInstance.Feedback);
        }

        [Fact]
        public void ParameterisedAction()
        {
            var testInstance = new TestParameterisedAction();
            testInstance.Action(3);
            Assert.Equal(3, testInstance.Feedback);

            using (Jaz.SpyOn(testInstance, nameof(TestParameterisedAction.Action)))
            {
                testInstance.Action(7);
            }

            Assert.Equal(3, testInstance.Feedback);
            testInstance.Action(4);
            Assert.Equal(7, testInstance.Feedback);
        }

        [Fact]
        public void ParameterlessFunc()
        {
            var testInstance = new TestParameterlessFunc();
            Assert.Equal(1, testInstance.Func());

            using (Jaz.SpyOn(testInstance, nameof(TestParameterlessFunc.Func)))
            {
                Assert.Equal(0, testInstance.Func());
            }

            Assert.Equal(2, testInstance.Func());
        }

        [Fact]
        public void ParameterisedFunc()
        {
            var testInstance = new TestParameterisedFunc();
            Assert.Equal(3, testInstance.Func(3));

            var spy = Jaz.SpyOn(testInstance, nameof(TestParameterisedFunc.Func));
            Assert.Equal(0, testInstance.Func(3));
            //spy.Dispose();

            //Assert.Equal(7, testInstance.Func(4));
        }

        private class TestParameterlessAction
        {
            public int Feedback { get; set; }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public void Action()
            {
                Feedback++;
            }
        }

        private class TestParameterisedAction
        {
            public int Feedback { get; set; }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public void Action(int amount)
            {
                Feedback += amount;
            }
        }

        private class TestParameterlessFunc
        {
            public int Feedback { get; set; }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public int Func()
            {
                return ++Feedback;
            }
        }

        private class TestParameterisedFunc
        {
            public int Feedback { get; set; }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public int Func(int amount)
            {
                return Feedback += amount;
            }
        }
    }
}
