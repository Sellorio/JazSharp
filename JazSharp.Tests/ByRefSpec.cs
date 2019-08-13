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
                var result = value.Length == 1;
                value = result ? new string(new[] { value[0] }) : null;
                return result;
            }

            public bool Parse(string value, out char character)
            {
                character = value.FirstOrDefault();
                return value.Length == 1;
            }
        }

        private class TestException : Exception
        {
        }
    }
}
