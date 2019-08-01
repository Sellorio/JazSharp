using JazSharp.Testing;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace JazSharp.TestAdapter
{
    internal static class ExtensionsForTest
    {
        public static TestCase ToTestCase(this Test test)
        {
            return new TestCase(test.TestClass.FullName + "._", TestAdapterConstants.ExecutorUri, test.AssemblyFilename)
            {
                CodeFilePath = test.SourceFilename,
                LineNumber = test.LineNumber,
                DisplayName = test.FullName
            };
        }

        public static bool IsForTestCase(this Test test, TestCase testCase)
        {
            return
                test.SourceFilename == testCase.CodeFilePath &&
                test.LineNumber == testCase.LineNumber &&
                test.FullName == testCase.DisplayName;
        }
    }
}
