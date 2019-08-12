using JazSharp.Expectations;
using JazSharp.Spies;
using System.Diagnostics.CodeAnalysis;

namespace JazSharp.Tests.Expectations
{
    public class SpyExpectSpec : Spec
    {
        public SpyExpectSpec()
        {
            Describe<SpyExpect>(() =>
            {
                TestSubject testSubject = null;
                Spy spy = null;

                BeforeEach(() =>
                {
                    testSubject = new TestSubject();
                    spy = Jaz.SpyOn(testSubject, nameof(testSubject.Action));
                });

                Describe(nameof(SpyExpect.Not), () =>
                {
                    It("should invert the check.", () =>
                    {
                        Expect(spy).Not.ToHaveBeenCalled();
                    });
                });

                Describe(nameof(SpyExpect.ToHaveBeenCalled), () =>
                {
                    It("should fail if the spy was not called.", () =>
                    {
                        Expect(() => Expect(spy).ToHaveBeenCalled()).ToThrow<JazExpectationException>();
                    });

                    It("should pass if the spy was called.", () =>
                    {
                        testSubject.Action(1, 2.0, "three");
                        Expect(spy).ToHaveBeenCalled();
                    });

                    Describe("with Not", () =>
                    {
                        It("should invert the check.", () =>
                        {
                            Expect(spy).Not.ToHaveBeenCalled();

                            testSubject.Action(1, 2.0, "three");
                            Expect(() => Expect(spy).Not.ToHaveBeenCalled()).ToThrow<JazExpectationException>();
                        });
                    });
                });

                Describe(nameof(SpyExpect.ToHaveBeenCalledTimes), () =>
                {
                    It("should fail if the spy was called less than the expected number of times.", () =>
                    {
                        Expect(() => Expect(spy).ToHaveBeenCalledTimes(1)).ToThrow<JazExpectationException>();
                    });

                    It("should pass if the spy was called the expected number of times.", () =>
                    {
                        testSubject.Action(1, 2.0, "three");
                        Expect(spy).ToHaveBeenCalledTimes(1);
                    });

                    It("should fail if the spy was called more than the expected number of times.", () =>
                    {
                        testSubject.Action(1, 2.0, "three");
                        testSubject.Action(1, 2.0, "three");
                        Expect(() => Expect(spy).ToHaveBeenCalledTimes(1)).ToThrow<JazExpectationException>();
                    });

                    Describe("with Not", () =>
                    {
                        It("should invert the check.", () =>
                        {
                            Expect(spy).Not.ToHaveBeenCalledTimes(1);

                            testSubject.Action(1, 2.0, "three");
                            Expect(() => Expect(spy).Not.ToHaveBeenCalledTimes(1)).ToThrow<JazExpectationException>();

                            testSubject.Action(1, 2.0, "three");
                            Expect(spy).Not.ToHaveBeenCalledTimes(1);
                        });
                    });
                });

                Describe(nameof(SpyExpect.ToHaveBeenCalledWith), () =>
                {
                    It("should fail if the spy was not called.", () =>
                    {
                        Expect(() => Expect(spy).ToHaveBeenCalledWith(1, 2.0, "three")).ToThrow<JazExpectationException>();
                    });

                    It("should fail if the spy was not called with the expected parameters.", () =>
                    {
                        testSubject.Action(1, 2.0, "tree");
                        Expect(() => Expect(spy).ToHaveBeenCalledWith(1, 2.0, "three")).ToThrow<JazExpectationException>();
                    });

                    It("should pass if the spy was called with the expected parameters.", () =>
                    {
                        testSubject.Action(1, 2.0, "three");
                        Expect(spy).ToHaveBeenCalledWith(1, 2.0, "three");
                    });

                    Describe("with Not", () =>
                    {
                        It("should invert the check.", () =>
                        {
                            Expect(spy).Not.ToHaveBeenCalledWith(1, 2.0, "three");

                            testSubject.Action(1, 2.0, "tree");
                            Expect(spy).Not.ToHaveBeenCalledWith(1, 2.0, "three");

                            testSubject.Action(1, 2.0, "three");
                            Expect(() => Expect(spy).Not.ToHaveBeenCalledWith(1, 2.0, "three")).ToThrow<JazExpectationException>();
                        });
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
