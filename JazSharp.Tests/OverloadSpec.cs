using System;

namespace JazSharp.Tests
{
    public class OverloadSpec : Spec
    {
        public OverloadSpec()
        {
            Describe("Method with overloads", () =>
            {
                It("should have all overloads covered by the one spy.", () =>
                {
                    var spy = Jaz.SpyOn(typeof(TestSubject), nameof(TestSubject.PositiveOne)).And.ReturnValues(3, 2, 1);

                    Expect(TestSubject.PositiveOne()).ToBe(3);
                    Expect(TestSubject.PositiveOne(0)).ToBe(2);
                    Expect(TestSubject.PositiveOne(0)).ToBe(1);
                });

                It("should record calls to both overloads.", () =>
                {
                    var spy = Jaz.SpyOn(typeof(TestSubject), nameof(TestSubject.Write));

                    TestSubject.Write(1, 2, 3);
                    TestSubject.Write(3);
                    TestSubject.Write(2, 3);

                    Expect(spy).ToHaveBeenCalledWith(2, 3);
                    Expect(spy).ToHaveBeenCalledWith(3);
                    Expect(spy).ToHaveBeenCalledWith(1, 2, 3);
                });
            });
        }

        private static class TestSubject
        {
            public static int PositiveOne()
            {
                return PositiveOne(0);
            }

            public static int PositiveOne(int from)
            {
                return from + 1;
            }

            public static void Write(int p)
            {
                Console.Write(p);
            }

            public static void Write(int p, int p2)
            {
                Console.Write(p);
                Console.Write(p2);
            }

            public static void Write(int p, int p2, int p3)
            {
                Console.Write(p);
                Console.Write(p2);
                Console.Write(p3);
            }
        }
    }
}
