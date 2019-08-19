using JazSharp.Testing;
using Mono.Cecil;
using System.Reflection;

namespace JazSharp.ManualTest
{
    class Program
    {
        static void Main(string[] _)
        {
            var info = AssemblyDefinition.ReadAssembly(Assembly.GetExecutingAssembly().Location).MainModule.GetType("JazSharp.ManualTest.Program").Methods;
            
            using (var testCollection = TestCollection.FromSources(new[] { @"C:\Users\seamillo\source\repos\JazSharp\JazSharp.Tests\bin\Debug\netcoreapp2.2\JazSharp.Tests.dll" }))
            using (var testRun = testCollection.CreateTestRun())
            {
                var result = testRun.ExecuteAsync().Result;
            }
        }

        public string Test<T>(T p)
            where T : struct
        {
            return p.ToString();
        }

        public override string ToString()
        {
            return ToString() + "x";
        }
    }
}