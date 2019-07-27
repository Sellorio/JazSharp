using System;

namespace JazSharp.ManualTest
{
    class Program
    {
        static void Main(string[] _)
        {
            Do();
        }

        static void Do()
        {
            var instance = new Test();
            var str = "a string";
            var spy = Jaz.SpyOn(str, nameof(string.GetHashCode), new Type[0]);
            Jaz.Invoke(() => instance.Method(str));
        }

        public class Test
        {
            public string _value;

            public int Method(string value)
            {
                _value = value;
                return _value.GetHashCode();
            }
        }
    }
}

// Add support for:
//  * Calling external/interop/native methods without wrapper
//  * Handling virtual and abstract methods (use instance to get override implementation)

// And.Throw
// Jaz.CreateSpyObject<T>() (support interfaces (emit), abstract classes (emit + spies), concrete classes (spies))