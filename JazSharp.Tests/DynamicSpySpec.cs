using System;

namespace JazSharp.Tests
{
    public class DynamicSpySpec : Spec
    {
        public DynamicSpySpec()
        {
            Describe("Dynamic Spy", () =>
            {
                TestSubject testSubject = null;

                BeforeEach(() =>
                {
                    testSubject = new TestSubject();
                });

                It("should not allow CallThrough configuration.", () =>
                {
                    Jaz.CreateSpy(out var spy);
                    Expect(() => spy.And.CallThrough()).ToThrow<InvalidOperationException>();

                    var spyThen = spy.And.Throw<Exception>().Then;

                    Expect(() => spyThen.CallThrough()).ToThrow<InvalidOperationException>();
                });

                Describe("Action", () =>
                {
                    Describe("with no parameters", () =>
                    {
                        It("should record the call.", () =>
                        {
                            testSubject.EventAction0 += Jaz.CreateSpy(out var spy);
                            testSubject.InvokeAll();
                            Expect(spy).ToHaveBeenCalledWith();
                        });

                        It("should throw an exception as configured.", () =>
                        {
                            testSubject.EventAction0 += Jaz.CreateSpy(out var spy);
                            spy.And.Throw<TestException>();
                            Expect(() => testSubject.InvokeAll()).ToThrow<TestException>();
                        });
                    });

                    Describe("with 1 parameter", () =>
                    {
                        It("should record the call.", () =>
                        {
                            testSubject.EventAction1 += Jaz.CreateSpy<int>(out var spy);
                            testSubject.InvokeAll();
                            Expect(spy).ToHaveBeenCalledWith(1);
                        });

                        It("should throw an exception as configured.", () =>
                        {
                            testSubject.EventAction1 += Jaz.CreateSpy<int>(out var spy);
                            spy.And.Throw<TestException>();
                            Expect(() => testSubject.InvokeAll()).ToThrow<TestException>();
                        });
                    });

                    Describe("with 2 parameters", () =>
                    {
                        It("should record the call.", () =>
                        {
                            testSubject.EventAction2 += Jaz.CreateSpy<int, int>(out var spy);
                            testSubject.InvokeAll();
                            Expect(spy).ToHaveBeenCalledWith(1, 2);
                        });

                        It("should throw an exception as configured.", () =>
                        {
                            testSubject.EventAction2 += Jaz.CreateSpy<int, int>(out var spy);
                            spy.And.Throw<TestException>();
                            Expect(() => testSubject.InvokeAll()).ToThrow<TestException>();
                        });
                    });

                    Describe("with 3 parameters", () =>
                    {
                        It("should record the call.", () =>
                        {
                            testSubject.EventAction3 += Jaz.CreateSpy<int, int, int>(out var spy);
                            testSubject.InvokeAll();
                            Expect(spy).ToHaveBeenCalledWith(1, 2, 3);
                        });

                        It("should throw an exception as configured.", () =>
                        {
                            testSubject.EventAction3 += Jaz.CreateSpy<int, int, int>(out var spy);
                            spy.And.Throw<TestException>();
                            Expect(() => testSubject.InvokeAll()).ToThrow<TestException>();
                        });
                    });

                    Describe("with 4 parameters", () =>
                    {
                        It("should record the call.", () =>
                        {
                            testSubject.EventAction4 += Jaz.CreateSpy<int, int, int, int>(out var spy);
                            testSubject.InvokeAll();
                            Expect(spy).ToHaveBeenCalledWith(1, 2, 3, 4);
                        });

                        It("should throw an exception as configured.", () =>
                        {
                            testSubject.EventAction4 += Jaz.CreateSpy<int, int, int, int>(out var spy);
                            spy.And.Throw<TestException>();
                            Expect(() => testSubject.InvokeAll()).ToThrow<TestException>();
                        });
                    });

                    Describe("with 5 parameters", () =>
                    {
                        It("should record the call.", () =>
                        {
                            testSubject.EventAction5 += Jaz.CreateSpy<int, int, int, int, int>(out var spy);
                            testSubject.InvokeAll();
                            Expect(spy).ToHaveBeenCalledWith(1, 2, 3, 4, 5);
                        });

                        It("should throw an exception as configured.", () =>
                        {
                            testSubject.EventAction5 += Jaz.CreateSpy<int, int, int, int, int>(out var spy);
                            spy.And.Throw<TestException>();
                            Expect(() => testSubject.InvokeAll()).ToThrow<TestException>();
                        });
                    });
                });

                Describe("Func", () =>
                {
                    Describe("with no parameters", () =>
                    {
                        It("should record the call.", () =>
                        {
                            testSubject.EventFunc0 += Jaz.CreateSpyFunc<string>(out var spy);
                            var result = testSubject.InvokeAll();
                            Expect(spy).ToHaveBeenCalledWith();
                            Expect(result).ToBeDefault();
                        });

                        It("should throw an exception as configured.", () =>
                        {
                            testSubject.EventAction0 += Jaz.CreateSpy(out var spy);
                            spy.And.Throw<TestException>();
                            Expect(() => testSubject.InvokeAll()).ToThrow<TestException>();
                        });

                        It("should return the specified value.", () =>
                        {
                            testSubject.EventFunc0 += Jaz.CreateSpyFunc<string>(out var spy);
                            spy.And.ReturnValue("abc");
                            var result = testSubject.InvokeAll();
                            Expect(result).ToBe("abc");
                        });
                    });

                    Describe("with 1 parameter", () =>
                    {
                        It("should record the call.", () =>
                        {
                            testSubject.EventFunc1 += Jaz.CreateSpyFunc<int, string>(out var spy);
                            var result = testSubject.InvokeAll();
                            Expect(spy).ToHaveBeenCalledWith(1);
                            Expect(result).ToBeDefault();
                        });

                        It("should throw an exception as configured.", () =>
                        {
                            testSubject.EventFunc1 += Jaz.CreateSpyFunc<int, string>(out var spy);
                            spy.And.Throw<TestException>();
                            Expect(() => testSubject.InvokeAll()).ToThrow<TestException>();
                        });

                        It("should return the specified value.", () =>
                        {
                            testSubject.EventFunc1 += Jaz.CreateSpyFunc<int, string>(out var spy);
                            spy.And.ReturnValue("abc");
                            var result = testSubject.InvokeAll();
                            Expect(result).ToBe("abc");
                        });
                    });

                    Describe("with 2 parameter", () =>
                    {
                        It("should record the call.", () =>
                        {
                            testSubject.EventFunc2 += Jaz.CreateSpyFunc<int, int, string>(out var spy);
                            var result = testSubject.InvokeAll();
                            Expect(spy).ToHaveBeenCalledWith(1, 2);
                            Expect(result).ToBeDefault();
                        });

                        It("should throw an exception as configured.", () =>
                        {
                            testSubject.EventFunc2 += Jaz.CreateSpyFunc<int, int, string>(out var spy);
                            spy.And.Throw<TestException>();
                            Expect(() => testSubject.InvokeAll()).ToThrow<TestException>();
                        });

                        It("should return the specified value.", () =>
                        {
                            testSubject.EventFunc2 += Jaz.CreateSpyFunc<int, int, string>(out var spy);
                            spy.And.ReturnValue("abc");
                            var result = testSubject.InvokeAll();
                            Expect(result).ToBe("abc");
                        });
                    });

                    Describe("with 3 parameter", () =>
                    {
                        It("should record the call.", () =>
                        {
                            testSubject.EventFunc3 += Jaz.CreateSpyFunc<int, int, int, string>(out var spy);
                            var result = testSubject.InvokeAll();
                            Expect(spy).ToHaveBeenCalledWith(1, 2, 3);
                            Expect(result).ToBeDefault();
                        });

                        It("should throw an exception as configured.", () =>
                        {
                            testSubject.EventFunc3 += Jaz.CreateSpyFunc<int, int, int, string>(out var spy);
                            spy.And.Throw<TestException>();
                            Expect(() => testSubject.InvokeAll()).ToThrow<TestException>();
                        });

                        It("should return the specified value.", () =>
                        {
                            testSubject.EventFunc3 += Jaz.CreateSpyFunc<int, int, int, string>(out var spy);
                            spy.And.ReturnValue("abc");
                            var result = testSubject.InvokeAll();
                            Expect(result).ToBe("abc");
                        });
                    });

                    Describe("with 4 parameter", () =>
                    {
                        It("should record the call.", () =>
                        {
                            testSubject.EventFunc4 += Jaz.CreateSpyFunc<int, int, int, int, string>(out var spy);
                            var result = testSubject.InvokeAll();
                            Expect(spy).ToHaveBeenCalledWith(1, 2, 3, 4);
                            Expect(result).ToBeDefault();
                        });

                        It("should throw an exception as configured.", () =>
                        {
                            testSubject.EventFunc4 += Jaz.CreateSpyFunc<int, int, int, int, string>(out var spy);
                            spy.And.Throw<TestException>();
                            Expect(() => testSubject.InvokeAll()).ToThrow<TestException>();
                        });

                        It("should return the specified value.", () =>
                        {
                            testSubject.EventFunc4 += Jaz.CreateSpyFunc<int, int, int, int, string>(out var spy);
                            spy.And.ReturnValue("abc");
                            var result = testSubject.InvokeAll();
                            Expect(result).ToBe("abc");
                        });
                    });

                    Describe("with 4 parameter", () =>
                    {
                        It("should record the call.", () =>
                        {
                            testSubject.EventFunc5 += Jaz.CreateSpyFunc<int, int, int, int, int, string>(out var spy);
                            var result = testSubject.InvokeAll();
                            Expect(spy).ToHaveBeenCalledWith(1, 2, 3, 4, 5);
                            Expect(result).ToBeDefault();
                        });

                        It("should throw an exception as configured.", () =>
                        {
                            testSubject.EventFunc5 += Jaz.CreateSpyFunc<int, int, int, int, int, string>(out var spy);
                            spy.And.Throw<TestException>();
                            Expect(() => testSubject.InvokeAll()).ToThrow<TestException>();
                        });

                        It("should return the specified value.", () =>
                        {
                            testSubject.EventFunc5 += Jaz.CreateSpyFunc<int, int, int, int, int, string>(out var spy);
                            spy.And.ReturnValue("abc");
                            var result = testSubject.InvokeAll();
                            Expect(result).ToBe("abc");
                        });
                    });
                });
            });
        }

        private class TestSubject
        {
            public event Action EventAction0;
            public event Action<int> EventAction1;
            public event Action<int, int> EventAction2;
            public event Action<int, int, int> EventAction3;
            public event Action<int, int, int, int> EventAction4;
            public event Action<int, int, int, int, int> EventAction5;

            public event Func<string> EventFunc0;
            public event Func<int, string> EventFunc1;
            public event Func<int, int, string> EventFunc2;
            public event Func<int, int, int, string> EventFunc3;
            public event Func<int, int, int, int, string> EventFunc4;
            public event Func<int, int, int, int, int, string> EventFunc5;

            public string InvokeAll()
            {
                EventAction0?.Invoke();
                EventAction1?.Invoke(1);
                EventAction2?.Invoke(1, 2);
                EventAction3?.Invoke(1, 2, 3);
                EventAction4?.Invoke(1, 2, 3, 4);
                EventAction5?.Invoke(1, 2, 3, 4, 5);

                return
                    EventFunc0?.Invoke() ??
                    EventFunc1?.Invoke(1) ??
                    EventFunc2?.Invoke(1, 2) ??
                    EventFunc3?.Invoke(1, 2, 3) ??
                    EventFunc4?.Invoke(1, 2, 3, 4) ??
                    EventFunc5?.Invoke(1, 2, 3, 4, 5);
            }
        }

        private class TestException : Exception
        {
        }
    }
}
