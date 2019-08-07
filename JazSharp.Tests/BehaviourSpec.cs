using System;

namespace JazSharp.Tests
{
    public class BehaviourSpec : Spec
    {
        public BehaviourSpec()
        {
            var testSubject = new TestSubject();

            Describe("Behaviour", () =>
            {
                Describe("of Return Value", () =>
                {
                    It("should return the provided return value.", () =>
                    {
                        Jaz.SpyOn(testSubject, nameof(testSubject.Func)).And.ReturnValue("abc");
                        var result = testSubject.Func();
                        Expect(result).ToBe("abc");
                    });

                    It("should cycle through return values once when using return values.", () =>
                    {
                        Jaz.SpyOn(testSubject, nameof(testSubject.Func)).And.ReturnValues("a", "b", "c");
                        var result = testSubject.Func();
                        Expect(result).ToBe("a");
                        result = testSubject.Func();
                        Expect(result).ToBe("b");
                        result = testSubject.Func();
                        Expect(result).ToBe("c");
                    });
                });

                Describe("of Call Through", () =>
                {
                    It("should call through to the original implementation.", () =>
                    {
                        Jaz.SpyOn(testSubject, nameof(testSubject.Func)).And.CallThrough();
                        var result = testSubject.Func();
                        Expect(result).ToBe("xyz");
                    });
                });

                Describe("of Default", () =>
                {
                    It("should return the default value of the function.", () =>
                    {
                        Jaz.SpyOn(testSubject, nameof(testSubject.Func));
                        var result = testSubject.Func();
                        Expect(result).ToBeDefault();
                    });

                    It("should have no logic executed for an action.", () =>
                    {
                        Jaz.SpyOn(testSubject, nameof(testSubject.Action));
                        testSubject.Action();
                        Expect(testSubject.Value).ToBeDefault();
                    });
                });

                Describe("of Throw", () =>
                {
                    It("should throw the given exception.", () =>
                    {
                        var exception = new System.Exception();
                        Jaz.SpyOn(testSubject, nameof(testSubject.Action)).And.Throw(exception);
                        
                        try
                        {
                            testSubject.Action();
                            Fail("Expected an exception to be thrown.");
                        }
                        catch (Exception ex)
                        {
                            if (ex != exception)
                            {
                                throw;
                            }
                        }
                    });

                    It("should throw an exception of the given type.", () =>
                    {
                        Jaz.SpyOn(testSubject, nameof(testSubject.Action)).And.Throw<Exception>();

                        try
                        {
                            testSubject.Action();
                            Fail("Expected an exception to be thrown.");
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message != "Exception of type 'System.Exception' was thrown.")
                            {
                                throw;
                            }
                        }
                    });
                });

                Describe("in sequence", () =>
                {
                    It("should execute forever if added using And.", () =>
                    {
                        Jaz.SpyOn(testSubject, nameof(testSubject.Func)).And.ReturnValue("abc");
                        var result = testSubject.Func();
                        Expect(result).ToBe("abc");
                        result = testSubject.Func();
                        Expect(result).ToBe("abc");
                        result = testSubject.Func();
                        Expect(result).ToBe("abc");
                        result = testSubject.Func();
                        Expect(result).ToBe("abc");
                    });

                    It("should clear existing behaviours if added using And.", () =>
                    {
                        var spy = Jaz.SpyOn(testSubject, nameof(testSubject.Func));
                        spy.And.Throw<Exception>();
                        spy.And.ReturnValue("abc");
                        var result = testSubject.Func();
                        Expect(result).ToBe("abc");
                    });

                    It("should execute once after default if added using Then.", () =>
                    {
                        Jaz.SpyOn(testSubject, nameof(testSubject.Func)).Then.ReturnValue("abc");
                        var result = testSubject.Func();
                        Expect(result).ToBeDefault();
                        result = testSubject.Func();
                        Expect(result).ToBe("abc");

                        try
                        {
                            testSubject.Func();
                            Fail("Expected spy exception after running out of behaviours.");
                        }
                        catch (JazSpyException)
                        {
                        }
                    });

                    It("should execute once if Once is specified.", () =>
                    {
                        Jaz.SpyOn(testSubject, nameof(testSubject.Func)).And.ReturnValue("abc").Once();
                        var result = testSubject.Func();
                        Expect(result).ToBe("abc");

                        try
                        {
                            testSubject.Func();
                            Fail("Expected spy exception after running out of behaviours.");
                        }
                        catch (JazSpyException)
                        {
                        }
                    });

                    It("should excute twice if Twice is specified.", () =>
                    {
                        Jaz.SpyOn(testSubject, nameof(testSubject.Func)).And.ReturnValue("abc").Twice();
                        var result = testSubject.Func();
                        Expect(result).ToBe("abc");
                        result = testSubject.Func();
                        Expect(result).ToBe("abc");

                        try
                        {
                            testSubject.Func();
                            Fail("Expected spy exception after running out of behaviours.");
                        }
                        catch (JazSpyException)
                        {
                        }
                    });

                    It("should execute the number of times specified in Times.", () =>
                    {
                        Jaz.SpyOn(testSubject, nameof(testSubject.Func)).And.ReturnValue("abc").Times(3);
                        var result = testSubject.Func();
                        Expect(result).ToBe("abc");
                        result = testSubject.Func();
                        Expect(result).ToBe("abc");
                        result = testSubject.Func();
                        Expect(result).ToBe("abc");

                        try
                        {
                            testSubject.Func();
                            Fail("Expected spy exception after running out of behaviours.");
                        }
                        catch (JazSpyException)
                        {
                        }
                    });

                    It("should execute forever if Forever is specified.", () =>
                    {
                        Jaz.SpyOn(testSubject, nameof(testSubject.Func)).And.ReturnValue("abc").Forever();
                        var result = testSubject.Func();
                        Expect(result).ToBe("abc");
                        result = testSubject.Func();
                        Expect(result).ToBe("abc");
                        result = testSubject.Func();
                        Expect(result).ToBe("abc");
                    });
                });
            });
        }

        private class TestSubject
        {
            public int Value;

            public string Func()
            {
                return "xyz";
            }

            public void Action()
            {
                Value++;
            }
        }
    }
}
