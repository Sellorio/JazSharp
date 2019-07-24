using System;

namespace JazSharp.ManualTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var instance = new Te();
            var spy = Jaz.SpyOn(instance, x => x.Value).Setter;

            instance.Value = "test";

            Jaz.Expect(spy).ToHaveBeenCalledWith("test");
            spy.And.CallThrough();

            instance.Value = "test2";

            Console.ReadKey();
        }

        private class Te
        {
            public static string DefaultValue { get; set; }
            public string Value { get; set; }
        }
    }
}

// DeepCompare to use IEquatable
// Jaz.Any<T>()
// Expect ToEqual (DeepCompare)
// Jaz.CreateSpyObject<T>() (support interfaces (emit), abstract classes (emit + spies), concrete classes (spies))