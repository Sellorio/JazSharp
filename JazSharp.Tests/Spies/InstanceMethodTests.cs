using System;

namespace JazSharp.Tests.Spies
{
    public class InstanceMethodTests : Spec
    {
        public InstanceMethodTests()
        {
            Describe("Jaz", () =>
            {
                It("should discover this test.", () =>
                {
                    var test = new Test();
                    test.GetAndRunV(v =>
                    {
                        Console.WriteLine(v);
                    });
                });
            });
        }

        public class Test
        {
            public string GetV()
            {
                return "V";
            }

            public void GetAndRunV(Action<string> run)
            {
                var v = GetV();
                run(v);
            }
        }
    }
}
