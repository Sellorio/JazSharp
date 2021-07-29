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
                    var dict = new Dictionary<string, string> { { "A", "1" } };
                    var key = dict.Keys.ToList()[0];
                    Expect(key).ToBe("A");
                });

                It("can get int key from dict", () =>
                {
                    var dict = new Dictionary<int, string> { { 1, "A" } };
                    var key = dict.Keys.ToList()[0];
                    Expect(key).ToBe(1);
                });
            });
        }
    }
}
