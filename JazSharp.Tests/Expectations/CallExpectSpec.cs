using JazSharp.Expectations;
using System;

namespace JazSharp.Tests.Expectations
{
    public class CallExpectSpec : Spec
    {
        public CallExpectSpec()
        {
            Describe<CallExpect>(() =>
            {
                Describe(nameof(CallExpect.ToThrow), () =>
                {
                    It("should pass if an exception was thrown.", () =>
                    {
                        Expect(() => throw new ParentException()).ToThrow<ParentException>();
                    });

                    It("should fail if an inheriting exception was thrown.", () =>
                    {
                        Expect(() => Expect(() => throw new ChildException()).ToThrow<ParentException>()).ToThrow<JazExpectationException>();
                    });

                    It("should return the thrown exception.", () =>
                    {
                        var exception = new ParentException();
                        var thrownException = Expect(() => throw exception).ToThrow<ParentException>();

                        Expect(thrownException).ToBe(exception);
                    });

                    Describe("with Not", () =>
                    {
                        It("should invert the check.", () =>
                        {
                            Expect(() => { }).Not.ToThrow<ParentException>();
                            Expect(() => throw new ChildException()).Not.ToThrow<ParentException>();
                        });
                    });
                });
            });
        }

        private class ChildException : ParentException
        {
        }

        private class ParentException : Exception
        {
        }
    }
}
