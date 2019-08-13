using JazSharp.Testing;
using Mono.Cecil;

namespace JazSharp.ManualTest
{
    class Program
    {
        static void Main(string[] _)
        {
            using (var testCollection = TestCollection.FromSources(new[] { @"C:\Users\seamillo\source\repos\JazSharp\JazSharp.Tests\bin\Debug\netcoreapp2.2\JazSharp.Tests.dll" }))
            using (var testRun = testCollection.CreateTestRun())
            {
                var result = testRun.ExecuteAsync().Result;
            }
            //var info = AssemblyDefinition.ReadAssembly(System.Reflection.Assembly.GetExecutingAssembly().Location).MainModule.GetType("JazSharp.ManualTest.Program").Methods;
        }

        public static void ParamsCaller()
        {
            ParamsFunc("v1");
        }

        public static void ParamsFunc(params string[] input)
        {
            throw new System.NotImplementedException();
        }
    }
}