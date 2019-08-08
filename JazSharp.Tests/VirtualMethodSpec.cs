using System;
using JazSharp.Tests.ForReference;

namespace JazSharp.Tests
{
    public class VirtualMethodSpec : Spec
    {
        public VirtualMethodSpec()
        {
            Describe("Virtual Method", () =>
            {
                Describe("overrides", () =>
                {
                    It("should call spy wrapper on method calls.", () =>
                    {
                        var action = (Action)(() => { });
                        var testSubject = (TestSubjectBase)new TestSubject();

                        var actionSpy = Jaz.SpyOn(action, nameof(action.Invoke));

                        testSubject.Call(action);

                        Expect(actionSpy).ToHaveBeenCalled();
                    });
                });
            });
        }

        private class TestSubject : TestSubjectBase
        {
            public override void Call(Action action)
            {
                action.Invoke();
            }
        }
    }
}
