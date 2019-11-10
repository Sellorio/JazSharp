using System.Collections.Generic;
using System.Linq;

namespace JazSharp.Tests
{
    class DictionarySpec : Spec
    {
        public DictionarySpec()
        {
            Describe("I", () =>
            {
                It("can get string key from dict", () =>
                {
                    StringKeyTest();
                });

                It("can get int key from dict", () =>
                {
                    IntKeyTest();
                });
            });
        }

        private static void IntKeyTest()
        {
            var dict = new Dictionary<int, string> { { 1, "A" } };

            var result = dict.FirstOrDefault();
            var key = result.Key;

            Expect(key).ToBe(1);
        }

        private static void StringKeyTest()
        {
            var dict = new Dictionary<string, string> { { "A", "1" } };

            var result = dict.FirstOrDefault();
            var key = result.Key;

            Expect(key).ToBe("A");
        }
    }
}
