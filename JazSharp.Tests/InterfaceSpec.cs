using JazSharp.Tests.ForReference;
using System;

namespace JazSharp.Tests
{
    public class InterfaceSpec : Spec
    {
        public InterfaceSpec()
        {
            Describe("Interface", () =>
            {
                Describe("implementations", () =>
                {
                    It("should call spy wrapper on method calls.", () =>
                    {
                        var action = (Action)(() => { });
                        var testSubject = (ITestSubject)new TestSubject();

                        var actionSpy = Jaz.SpyOn(action, nameof(action.Invoke));

                        testSubject.Call(action);

                        Expect(actionSpy).ToHaveBeenCalled();
                    });
                });
            });
        }

        private class TestSubject : ITestSubject
        {
            public void Call(Action action)
            {
                action.Invoke();
            }
        }
    }
}
