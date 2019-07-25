using System.Diagnostics;
using System.Linq;

namespace JazSharp
{
    internal static class TestScopeHelper
    {
        private static readonly string[] IgnoredAssemblyNames =
        {
            "xunit.core",
            "xunit.execution.dotnet",
            "System.Private.CoreLib",
            "System.Threading.Thread"
        };

        internal static string GetTestName()
        {
            var testMethod =
                new StackTrace()
                    .GetFrames()
                    .Where(x => x.HasMethod())
                    .Select(x => x.GetMethod())
                    .Last(x => !IgnoredAssemblyNames.Contains(x.DeclaringType.Assembly.GetName().Name));

            return testMethod.DeclaringType.FullName + "." + testMethod.Name;
        }
    }
}
