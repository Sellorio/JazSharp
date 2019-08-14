using System;

namespace JazSharp.Tests
{
    public class GenericInstanceMethodSpec : Spec
    {
        public GenericInstanceMethodSpec()
        {
            Describe("A generic instance action", () =>
            {
                var subject = new TestSubject();

                It("should call through by default.", () =>
                {
                    subject.Iterate(3);
                    Expect(subject.Value).ToBe(3);
                });

                It("should do nothing if spied on.", () =>
                {
                    Jaz.SpyOn(subject, nameof(subject.Iterate));
                    subject.Iterate(3);
                    Expect(subject.Value).ToBeDefault();
                });

                It("should call through as configured.", () =>
                {
                    Jaz.SpyOn(subject, nameof(subject.Iterate)).And.CallThrough();
                    subject.Iterate(3);
                    Expect(subject.Value).ToBe(3);
                });

                It("should throw an exception as configured.", () =>
                {
                    var exception = new TestException();
                    Jaz.SpyOn(subject, nameof(subject.Iterate)).And.Throw(exception);
                    var thrown = Expect(() => subject.Iterate(3)).ToThrow<TestException>();
                    Expect(thrown).ToBe(exception);
                });
            });

            Describe("A generic instance function", () =>
            {
                var subject = new TestSubject();

                It("should call through by default.", () =>
                {
                    var result = subject.Add5(3);
                    Expect(result).ToBe(8);
                });

                It("should do nothing if spied on.", () =>
                {
                    Jaz.SpyOn(subject, nameof(subject.Add5));
                    var result = subject.Add5(3);
                    Expect(result).ToBeDefault();
                });

                It("should return the configured value.", () =>
                {
                    Jaz.SpyOn(subject, nameof(subject.Add5)).And.ReturnValue(1);
                    var result = subject.Add5(3);
                    Expect(result).ToBe(1);
                });

                It("should return the configured values in sequence.", () =>
                {
                    Jaz.SpyOn(subject, nameof(subject.Add5)).And.ReturnValues(1, 2, 3, 4);
                    var result = subject.Add5(3);
                    Expect(result).ToBe(1);
                    result = subject.Add5(3);
                    Expect(result).ToBe(2);
                    result = subject.Add5(3);
                    Expect(result).ToBe(3);
                    result = subject.Add5(3);
                    Expect(result).ToBe(4);
                });

                It("should call through as configured.", () =>
                {
                    Jaz.SpyOn(subject, nameof(subject.Add5)).And.CallThrough();
                    var result = subject.Add5(3);
                    Expect(result).ToBe(8);
                });

                It("should throw an exception as configured.", () =>
                {
                    var exception = new TestException();
                    Jaz.SpyOn(subject, nameof(subject.Add5)).And.Throw(exception);
                    var thrown = Expect(() => subject.Add5(3)).ToThrow<TestException>();
                    Expect(thrown).ToBe(exception);
                });

                It("should support calling a generic method from a generic method.", () =>
                {
                    var testSubject = new TestSubject();
                    var result = testSubject.CallFunc(() => 5);
                    Expect(result).ToBe(5);
                });

                It("should support calling a generic method from a generic class.", () =>
                {
                    var testSubject = new TestSubject<int>();
                    var result = testSubject.CallFunc(() => 5);
                    Expect(result).ToBe(5);
                });

                It("should support calling a generic class from a generic method.", () =>
                {
                    var testSubject = new TestSubject();
                    var result = testSubject.CallFunc2(() => 5);
                    Expect(result).ToBe(5);
                });

                It("should support calling a method on a nested class in a generic class.", () =>
                {
                    var testSubject = new TestSubject<int>.TestSubjectChild();
                    var result = testSubject.CallFunc(() => 5);
                    Expect(result).ToBe(5);
                });
            });
        }

        private class TestSubject
        {
            public int Value;

            public void Iterate<TAmount>(TAmount amount)
            {
                if (amount is int v)
                {
                    Value += v;
                }
            }

            public TVA Add5<TVA>(TVA value)
            {
                if (typeof(TVA) == typeof(int))
                {
                    return (TVA)(object)((int)(object)value + 5);
                }

                return default;
            }

            public TVB CallFunc<TVB>(Func<TVB> func)
            {
                return func.Invoke();
            }

            public TVC CallFunc2<TVC>(Func<TVC> func)
            {
                return (TVC)new TestSubject<Func<TVC>>().CallFunc2(func);
            }
        }

        private class TestSubject<TVD>
        {
            public TVD CallFunc(Func<TVD> func)
            {
                return func.Invoke();
            }

            public object CallFunc2(TVD func)
            {
                return ((Delegate)(object)func).Method.Invoke(((Delegate)(object)func).Target, new object[0]);
            }

            public class TestSubjectChild
            {
                public TVD CallFunc(Func<TVD> func)
                {
                    return func.Invoke();
                }
            }
        }

        private class TestException : Exception
        {
        }
    }
}
