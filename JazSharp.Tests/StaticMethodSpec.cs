using System;

namespace JazSharp.Tests
{
    public class StaticMethodSpec : Spec
    {
        public StaticMethodSpec()
        {
            Describe("A static action", () =>
            {
                BeforeEach(() =>
                {
                    TestSubject.Value = 0;
                });

                It("should call through by default.", () =>
                {
                    TestSubject.Iterate(3);
                    Expect(TestSubject.Value).ToBe(3);
                });

                It("should do nothing if spied on.", () =>
                {
                    Jaz.SpyOn(typeof(TestSubject), nameof(TestSubject.Iterate));
                    TestSubject.Iterate(3);
                    Expect(TestSubject.Value).ToBeDefault();
                });

                It("should call through as configured.", () =>
                {
                    Jaz.SpyOn(typeof(TestSubject), nameof(TestSubject.Iterate)).And.CallThrough();
                    TestSubject.Iterate(3);
                    Expect(TestSubject.Value).ToBe(3);
                });

                It("should throw an exception as configured.", () =>
                {
                    var exception = new TestException();
                    Jaz.SpyOn(typeof(TestSubject), nameof(TestSubject.Iterate)).And.Throw(exception);
                    var thrown = Expect(() => TestSubject.Iterate(3)).ToThrow<TestException>();
                    Expect(thrown).ToBe(exception);
                });

                // the only non-generic spy entry point
                Describe("with no parameters", () =>
                {
                    It("should call through by default.", () =>
                    {
                        TestSubject.Increment();
                        Expect(TestSubject.Value).ToBe(1);
                    });

                    It("should do nothing if spied on.", () =>
                    {
                        Jaz.SpyOn(typeof(TestSubject), nameof(TestSubject.Increment));
                        TestSubject.Increment();
                        Expect(TestSubject.Value).ToBeDefault();
                    });

                    It("should call through as configured.", () =>
                    {
                        Jaz.SpyOn(typeof(TestSubject), nameof(TestSubject.Increment)).And.CallThrough();
                        TestSubject.Increment();
                        Expect(TestSubject.Value).ToBe(1);
                    });

                    It("should throw an exception as configured.", () =>
                    {
                        var exception = new TestException();
                        Jaz.SpyOn(typeof(TestSubject), nameof(TestSubject.Increment)).And.Throw(exception);
                        var thrown = Expect(() => TestSubject.Increment()).ToThrow<TestException>();
                        Expect(thrown).ToBe(exception);
                    });
                });
            });

            Describe("A static function", () =>
            {
                It("should call through by default.", () =>
                {
                    var result = TestSubject.Add5(3);
                    Expect(result).ToBe(8);
                });

                It("should do nothing if spied on.", () =>
                {
                    Jaz.SpyOn(typeof(TestSubject), nameof(TestSubject.Add5));
                    var result = TestSubject.Add5(3);
                    Expect(result).ToBeDefault();
                });

                It("should return the configured value.", () =>
                {
                    Jaz.SpyOn(typeof(TestSubject), nameof(TestSubject.Add5)).And.ReturnValue(1);
                    var result = TestSubject.Add5(3);
                    Expect(result).ToBe(1);
                });

                It("should return the configured values in sequence.", () =>
                {
                    Jaz.SpyOn(typeof(TestSubject), nameof(TestSubject.Add5)).And.ReturnValues(1, 2, 3, 4);
                    var result = TestSubject.Add5(3);
                    Expect(result).ToBe(1);
                    result = TestSubject.Add5(3);
                    Expect(result).ToBe(2);
                    result = TestSubject.Add5(3);
                    Expect(result).ToBe(3);
                    result = TestSubject.Add5(3);
                    Expect(result).ToBe(4);
                });

                It("should call through as configured.", () =>
                {
                    Jaz.SpyOn(typeof(TestSubject), nameof(TestSubject.Add5)).And.CallThrough();
                    var result = TestSubject.Add5(3);
                    Expect(result).ToBe(8);
                });

                It("should throw an exception as configured.", () =>
                {
                    var exception = new TestException();
                    Jaz.SpyOn(typeof(TestSubject), nameof(TestSubject.Add5)).And.Throw(exception);
                    var thrown = Expect(() => TestSubject.Add5(3)).ToThrow<TestException>();
                    Expect(thrown).ToBe(exception);
                });
            });
        }

        private static class TestSubject
        {
            public static int Value;

            public static void Iterate(int amount)
            {
                Value += amount;
            }

            public static void Increment()
            {
                Value++;
            }

            public static int Add5(int value)
            {
                return value + 5;
            }
        }

        private class TestException : Exception
        {
        }
    }
}
