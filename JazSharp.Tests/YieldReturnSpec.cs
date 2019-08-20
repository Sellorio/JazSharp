using System;
using System.Collections.Generic;
using System.Linq;

namespace JazSharp.Tests
{
    public class YieldReturnSpec : Spec
    {
        public YieldReturnSpec()
        {
            Describe("Method using yield return", () =>
            {
                It("should call through by default.", () =>
                {
                    var result = TestSubject.Method().ToList();
                    Expect(result).ToEqual(new[] { 1, 2, 3, 5, 7 });
                });

                It("should do nothing if spied on.", () =>
                {
                    Jaz.SpyOn(typeof(TestSubject), nameof(TestSubject.Method));
                    var result = TestSubject.Method();
                    Expect(result).ToBeDefault();
                });

                It("should call through as configured.", () =>
                {
                    Jaz.SpyOn(typeof(TestSubject), nameof(TestSubject.Method)).And.CallThrough();
                    var result = TestSubject.Method().ToList();
                    Expect(result).ToEqual(new[] { 1, 2, 3, 5, 7 });
                });

                It("should throw an exception as configured.", () =>
                {
                    Jaz.SpyOn(typeof(TestSubject), nameof(TestSubject.Method)).And.Throw<TestException>();
                    Expect(() => TestSubject.Method()).ToThrow<TestException>();
                });

                It("should return the configured value.", () =>
                {
                    Jaz.SpyOn(typeof(TestSubject), nameof(TestSubject.Method)).And.ReturnValue(new[] { 4, 6, 8, 9 });
                    var result = TestSubject.Method().ToList();
                    Expect(result).ToEqual(new[] { 4, 6, 8, 9 });
                });
            });
        }

        private static class TestSubject
        {
            public static IEnumerable<int> Method()
            {
                yield return 1;
                yield return 2;
                yield return 3;
                yield return 5;
                yield return 7;
            }
        }

        private class TestException : Exception
        {
        }
    }
}
