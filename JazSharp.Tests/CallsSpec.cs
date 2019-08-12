using System.Diagnostics.CodeAnalysis;

namespace JazSharp.Tests
{
    public class CallsSpec : Spec
    {
        public CallsSpec()
        {
            Describe("Calls", () =>
            {
                TestSubject testSubject = null;

                BeforeEach(() =>
                {
                    testSubject = new TestSubject();
                });

                It("should be empty for a new spy.", () =>
                {
                    var spy = Jaz.SpyOn(testSubject, nameof(testSubject.Action));
                    Expect(spy.Calls).ToBeEmpty();
                });

                It("should contain an ordered call log.", () =>
                {
                    var spy = Jaz.SpyOn(testSubject, nameof(testSubject.Action));
                    testSubject.Action(1, 2.0, "three");
                    testSubject.Action(2, 1.0, null);

                    Expect(spy.Calls).ToEqual(new[]
                    {
                        new
                        {
                            Arguments = new object[] { 1, 2.0, "three" }
                        },
                        new
                        {
                            Arguments = new object[] { 2, 1.0, null }
                        }
                    });
                });
            });
        }

        private class TestSubject
        {
            [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "test")]
            public void Action(int a, double b, string c)
            {
            }
        }
    }
}
