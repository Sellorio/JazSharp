using System;
using System.Linq;

namespace JazSharp.Tests
{
    public class ByRefSpec : Spec
    {
        public ByRefSpec()
        {
            TestSubject testSubject = null;

            Describe("Action with by ref parameter", () =>
            {
                float testValue = default;

                BeforeEach(() =>
                {
                    testSubject = new TestSubject();
                    testValue = 85.5f;
                });

                It("should call through by default.", () =>
                {
                    testSubject.PercentToDecimal(ref testValue);
                    Expect(testValue).ToBe(0.855f);
                });

                It("should do nothing if spied on.", () =>
                {
                    Jaz.SpyOn(testSubject, nameof(testSubject.PercentToDecimal));
                    testSubject.PercentToDecimal(ref testValue);
                    Expect(testValue).ToBe(85.5f);
                });

                It("should call through as configured.", () =>
                {
                    Jaz.SpyOn(testSubject, nameof(testSubject.PercentToDecimal)).And.CallThrough();
                    testSubject.PercentToDecimal(ref testValue);
                    Expect(testValue).ToBe(0.855f);
                });

                It("should throw an exception as configured.", () =>
                {
                    Jaz.SpyOn(testSubject, nameof(testSubject.PercentToDecimal)).And.Throw<TestException>();
                    Expect(() => testSubject.PercentToDecimal(ref testValue)).ToThrow<TestException>();
                });

                It("should change the ref parameter as configured.", () =>
                {
                    Jaz.SpyOn(testSubject, nameof(testSubject.PercentToDecimal)).And.DoNothing().ThenChangeParameter("value", 1.0f);
                    testSubject.PercentToDecimal(ref testValue);
                    Expect(testValue).ToBe(1.0f);
                });
            });

            Describe("Action with out parameter", () =>
            {
                float testValue = default;

                BeforeEach(() =>
                {
                    testSubject = new TestSubject();
                    testValue = 85.5f;
                });

                It("should call through by default.", () =>
                {
                    testSubject.PercentToDecimalPreserve(testValue, out var output);
                    Expect(output).ToBe(0.855f);
                });

                It("should do nothing if spied on.", () =>
                {
                    Jaz.SpyOn(testSubject, nameof(testSubject.PercentToDecimalPreserve));
                    testSubject.PercentToDecimalPreserve(testValue, out var output);
                    Expect(output).ToBeDefault();
                });

                It("should call through as configured.", () =>
                {
                    Jaz.SpyOn(testSubject, nameof(testSubject.PercentToDecimalPreserve)).And.CallThrough();
                    testSubject.PercentToDecimalPreserve(testValue, out var output);
                    Expect(output).ToBe(0.855f);
                });

                It("should throw an exception as configured.", () =>
                {
                    Jaz.SpyOn(testSubject, nameof(testSubject.PercentToDecimalPreserve)).And.Throw<TestException>();
                    Expect(() => testSubject.PercentToDecimalPreserve(testValue, out var output)).ToThrow<TestException>();
                });

                It("should change the ref parameter as configured.", () =>
                {
                    Jaz.SpyOn(testSubject, nameof(testSubject.PercentToDecimalPreserve)).And.DoNothing().ThenChangeParameter("decimal", 1.0f);
                    testSubject.PercentToDecimalPreserve(testValue, out var output);
                    Expect(output).ToBe(1.0f);
                });
            });

            Describe("Func with by ref parameter", () =>
            {
                string testValue = null;

                BeforeEach(() =>
                {
                    testSubject = new TestSubject();
                    testValue = "abc";
                });

                It("should call through by default.", () =>
                {
                    var result = testSubject.ToChar(ref testValue);
                    Expect(testValue).ToBe("a");
                    Expect(result).ToBeTrue();
                });

                It("should do nothing if spied on.", () =>
                {
                    Jaz.SpyOn(testSubject, nameof(testSubject.ToChar));
                    var result = testSubject.ToChar(ref testValue);
                    Expect(testValue).ToBe("abc");
                    Expect(result).ToBeFalse();
                });

                It("should call through as configured.", () =>
                {
                    Jaz.SpyOn(testSubject, nameof(testSubject.ToChar)).And.CallThrough();
                    var result = testSubject.ToChar(ref testValue);
                    Expect(testValue).ToBe("a");
                    Expect(result).ToBeTrue();
                });

                It("should throw an exception as configured.", () =>
                {
                    Jaz.SpyOn(testSubject, nameof(testSubject.ToChar)).And.Throw<TestException>();
                    Expect(() => testSubject.ToChar(ref testValue)).ToThrow<TestException>();
                });

                It("should change the ref parameter as configured.", () =>
                {
                    Jaz.SpyOn(testSubject, nameof(testSubject.ToChar)).And.DoNothing().ThenChangeParameter("value", "ab");
                    var result = testSubject.ToChar(ref testValue);
                    Expect(testValue).ToBe("ab");
                    Expect(result).ToBeFalse();
                });

                It("should return value as configured.", () =>
                {
                    Jaz.SpyOn(testSubject, nameof(testSubject.ToChar)).And.ReturnValue(true);
                    var result = testSubject.ToChar(ref testValue);
                    Expect(testValue).ToBe("abc");
                    Expect(result).ToBeTrue();
                });
            });

            Describe("Func with out parameter", () =>
            {
                string testValue = null;

                BeforeEach(() =>
                {
                    testSubject = new TestSubject();
                    testValue = "abc";
                });

                It("should call through by default.", () =>
                {
                    var result = testSubject.Parse(testValue, out var asChar);
                    Expect(asChar).ToBe('a');
                    Expect(result).ToBeTrue();
                });

                It("should do nothing if spied on.", () =>
                {
                    Jaz.SpyOn(testSubject, nameof(testSubject.Parse));
                    var result = testSubject.Parse(testValue, out var asChar);
                    Expect(asChar).ToBeDefault();
                    Expect(result).ToBeFalse();
                });

                It("should call through as configured.", () =>
                {
                    Jaz.SpyOn(testSubject, nameof(testSubject.Parse)).And.CallThrough();
                    var result = testSubject.Parse(testValue, out var asChar);
                    Expect(asChar).ToBe('a');
                    Expect(result).ToBeTrue();
                });

                It("should throw an exception as configured.", () =>
                {
                    Jaz.SpyOn(testSubject, nameof(testSubject.Parse)).And.Throw<TestException>();
                    Expect(() => testSubject.Parse(testValue, out var asChar)).ToThrow<TestException>();
                });

                It("should change the ref parameter as configured.", () =>
                {
                    Jaz.SpyOn(testSubject, nameof(testSubject.Parse)).And.DoNothing().ThenChangeParameter("character", 'z');
                    var result = testSubject.Parse(testValue, out var asChar);
                    Expect(asChar).ToBe('z');
                    Expect(result).ToBeFalse();
                });

                It("should return value as configured.", () =>
                {
                    Jaz.SpyOn(testSubject, nameof(testSubject.Parse)).And.ReturnValue(true);
                    var result = testSubject.Parse(testValue, out var asChar);
                    Expect(asChar).ToBeDefault();
                    Expect(result).ToBeTrue();
                });

                Describe("with generic arguments", () =>
                {
                    It("should call through by default.", () =>
                    {
                        var result = testSubject.Cast<string>(testValue, out var cast);
                        Expect(cast).ToBe(testValue);
                        Expect(result).ToBeTrue();
                    });

                    It("should do nothing if spied on.", () =>
                    {
                        Jaz.SpyOn(testSubject, nameof(testSubject.Cast));
                        var result = testSubject.Cast<string>(testValue, out var cast);
                        Expect(cast).ToBeDefault();
                        Expect(result).ToBeFalse();
                    });

                    It("should call through as configured.", () =>
                    {
                        Jaz.SpyOn(testSubject, nameof(testSubject.Cast)).And.CallThrough();
                        var result = testSubject.Cast<string>(testValue, out var cast);
                        Expect(cast).ToBe(testValue);
                        Expect(result).ToBeTrue();
                    });

                    It("should throw an exception as configured.", () =>
                    {
                        Jaz.SpyOn(testSubject, nameof(testSubject.Cast)).And.Throw<TestException>();
                        Expect(() => testSubject.Cast<string>(testValue, out var cast)).ToThrow<TestException>();
                    });

                    It("should change the ref parameter as configured.", () =>
                    {
                        Jaz.SpyOn(testSubject, nameof(testSubject.Cast)).And.DoNothing().ThenChangeParameter("cast", "z");
                        var result = testSubject.Cast<string>(testValue, out var cast);
                        Expect(cast).ToBe("z");
                        Expect(result).ToBeFalse();
                    });

                    It("should return value as configured.", () =>
                    {
                        Jaz.SpyOn(testSubject, nameof(testSubject.Cast)).And.ReturnValue(true);
                        var result = testSubject.Cast<string>(testValue, out var cast);
                        Expect(cast).ToBeDefault();
                        Expect(result).ToBeTrue();
                    });
                });

                Describe("in a generic type", () =>
                {
                    It("should call through by default.", () =>
                    {
                        var result = TestSubject<string>.Cast(testValue, out var cast);
                        Expect(cast).ToBe(testValue);
                        Expect(result).ToBeTrue();
                    });

                    It("should do nothing if spied on.", () =>
                    {
                        Jaz.SpyOn(typeof(TestSubject<string>), nameof(TestSubject<string>.Cast));
                        var result = TestSubject<string>.Cast(testValue, out var cast);
                        Expect(cast).ToBeDefault();
                        Expect(result).ToBeFalse();
                    });

                    It("should call through as configured.", () =>
                    {
                        Jaz.SpyOn(typeof(TestSubject<string>), nameof(TestSubject<string>.Cast)).And.CallThrough();
                        var result = TestSubject<string>.Cast(testValue, out var cast);
                        Expect(cast).ToBe(testValue);
                        Expect(result).ToBeTrue();
                    });

                    It("should throw an exception as configured.", () =>
                    {
                        Jaz.SpyOn(typeof(TestSubject<string>), nameof(TestSubject<string>.Cast)).And.Throw<TestException>();
                        Expect(() => TestSubject<string>.Cast(testValue, out var cast)).ToThrow<TestException>();
                    });

                    It("should change the ref parameter as configured.", () =>
                    {
                        Jaz.SpyOn(typeof(TestSubject<string>), nameof(TestSubject<string>.Cast)).And.DoNothing().ThenChangeParameter("cast", "z");
                        var result = TestSubject<string>.Cast(testValue, out var cast);
                        Expect(cast).ToBe("z");
                        Expect(result).ToBeFalse();
                    });

                    It("should return value as configured.", () =>
                    {
                        Jaz.SpyOn(typeof(TestSubject<string>), nameof(TestSubject<string>.Cast)).And.ReturnValue(true);
                        var result = TestSubject<string>.Cast(testValue, out var cast);
                        Expect(cast).ToBeDefault();
                        Expect(result).ToBeTrue();
                    });
                });
            });
        }

        private class TestSubject
        {
            public void PercentToDecimal(ref float value)
            {
                value /= 100;
            }

            public void PercentToDecimalPreserve(float percent, out float @decimal)
            {
                @decimal = percent / 100;
            }

            public bool ToChar(ref string value)
            {
                var result = value.Length != 1;
                value = value.Substring(0, 1);
                return result;
            }

            public bool Parse(string value, out char character)
            {
                character = value.FirstOrDefault();
                return value.Length != 1;
            }

            public bool Cast<TValue>(object value, out TValue cast)
                where TValue : class
            {
                cast = value as TValue;
                return cast != null;
            }
        }

        private class TestSubject<TValue>
            where TValue : class
        {
            public static bool Cast(object value, out TValue cast)
            {
                cast = value as TValue;
                return cast != null;
            }
        }

        private class TestException : Exception
        {
        }
    }
}
