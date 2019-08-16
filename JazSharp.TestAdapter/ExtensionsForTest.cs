using JazSharp.Testing;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace JazSharp.TestAdapter
{
    internal static class ExtensionsForTest
    {
        private static readonly HashAlgorithm Hasher = SHA1.Create();
        private static readonly Regex EscapeFullNameRegex = new Regex(@"\.(.)");

        public static TestCase ToTestCase(this Test test)
        {
            return new TestCase(test.TestClass.FullName + "." + EscapeTestFullName(test.FullName), TestAdapterConstants.ExecutorUri, test.Execution.Main.Method.Module.Assembly.Location)
            {
                Id = GuidFromString(test.FullName + ":" + test.SourceFilename + ":" + test.LineNumber),
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

        private static Guid GuidFromString(string data)
        {
            var hash = Hasher.ComputeHash(Encoding.Unicode.GetBytes(data));
            var b = new byte[16];
            Array.Copy(hash, b, 16);
            return new Guid(b);
        }

        private static string EscapeTestFullName(string fullName)
        {
            return EscapeFullNameRegex.Replace(fullName, "_$1");
        }
    }
}
