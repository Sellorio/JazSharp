using System;
using System.Threading.Tasks;

namespace JazSharp.Tests
{
    public class AsyncAwaitSpec : Spec
    {
        public AsyncAwaitSpec()
        {
            Describe("Async Await methods", () =>
            {
                Describe("from non-async method", () =>
                {
                    It("should call through by default.", () =>
                    {
                        TestSubject.InternalMethod(5).GetAwaiter().GetResult();
                    });
                });

                Describe("without a result", () =>
                {
                    It("should call through by default.", async () =>
                    {
                        await TestSubject.MethodOne(5);
                    });

                    // spies may be changed to return Task.CompletedTask in future
                    It("should do nothing if spied on.", () =>
                    {
                        Jaz.SpyOn(typeof(TestSubject), nameof(TestSubject.MethodOne));
                        var task = TestSubject.MethodOne(5);
                        Expect(task).ToBeDefault(); 
                    });

                    It("should call through as configured.", async () =>
                    {
                        Jaz.SpyOn(typeof(TestSubject), nameof(TestSubject.MethodOne)).And.CallThrough();
                        await TestSubject.MethodOne(5);
                    });

                    It("should throw an exception as configured.", async () =>
                    {
                        Jaz.SpyOn(typeof(TestSubject), nameof(TestSubject.MethodOne)).And.Throw<TestException>();
                        await ExpectAsync(() => TestSubject.MethodOne(5)).ToThrow<TestException>();
                    });

                    It("should throw an exception as configured.", async () =>
                    {
                        Jaz.SpyOn(typeof(TestSubject), nameof(TestSubject.MethodOne)).And.Throw<TestException>();
                        await ExpectAsync(async () => await TestSubject.MethodOne(5)).ToThrow<TestException>();
                    });
                });

                Describe("with a result", () =>
                {
                    It("should call through by default.", async () =>
                    {
                        var result = await TestSubject.MethodTwo(5);
                        Expect(result).ToBe("5");
                    });

                    // spies may be changed to return Task.CompletedTask in future
                    It("should do nothing if spied on.", () =>
                    {
                        Jaz.SpyOn(typeof(TestSubject), nameof(TestSubject.MethodTwo));
                        var task = TestSubject.MethodTwo(5);
                        Expect(task).ToBeDefault();
                    });

                    It("should call through as configured.", async () =>
                    {
                        Jaz.SpyOn(typeof(TestSubject), nameof(TestSubject.MethodTwo)).And.CallThrough();
                        var result = await TestSubject.MethodTwo(5);
                        Expect(result).ToBe("5");
                    });

                    It("should throw an exception as configured.", async () =>
                    {
                        Jaz.SpyOn(typeof(TestSubject), nameof(TestSubject.MethodOne)).And.Throw<TestException>();
                        await ExpectAsync(() => TestSubject.MethodOne(5)).ToThrow<TestException>();
                    });

                    It("should throw an exception as configured.", async () =>
                    {
                        Jaz.SpyOn(typeof(TestSubject), nameof(TestSubject.MethodOne)).And.Throw<TestException>();
                        await ExpectAsync(async () => await TestSubject.MethodOne(5)).ToThrow<TestException>();
                    });

                    It("should return the configured value.", async () =>
                    {
                        Jaz.SpyOn(typeof(TestSubject), nameof(TestSubject.MethodTwo)).And.ReturnValue(Task.FromResult("3"));
                        var result = await TestSubject.MethodTwo(5);
                        Expect(result).ToBe("3");
                    });
                });
            });
        }

        private static class TestSubject
        {
            public static async Task MethodOne(int value)
            {
                await InternalMethod(value);
            }

            public static async Task<string> MethodTwo(int value)
            {
                await InternalMethod(value);
                return value.ToString();
            }

            public static async Task InternalMethod(int value)
            {
                await Task.Run(() => { });
            }
        }

        private class TestException : Exception
        {
        }
    }
}
