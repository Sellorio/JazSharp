using System;
using System.Runtime.InteropServices;

namespace JazSharp.Tests
{
    public class ExternSpec : Spec
    {
        [DllImport("JazSharp.Tests.NativeLibrary.dll", EntryPoint = "GetInt", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetInt();

        public ExternSpec()
        {
            Describe("Extern methods", () =>
            {
                It("should call through by default.", () =>
                {
                    Expect(GetInt()).ToBe(50);
                });

                It("should do nothing if spied on.", () =>
                {
                    Jaz.SpyOn(typeof(ExternSpec), nameof(GetInt));
                    Expect(GetInt()).ToBeDefault();
                });

                It("should return the configured value.", () =>
                {
                    Jaz.SpyOn(typeof(ExternSpec), nameof(GetInt)).And.ReturnValue(1);
                    var result = GetInt();
                    Expect(result).ToBe(1);
                });

                It("should return the configured values in sequence.", () =>
                {
                    Jaz.SpyOn(typeof(ExternSpec), nameof(GetInt)).And.ReturnValues(1, 2, 3, 4);
                    var result = GetInt();
                    Expect(result).ToBe(1);
                    result = GetInt();
                    Expect(result).ToBe(2);
                    result = GetInt();
                    Expect(result).ToBe(3);
                    result = GetInt();
                    Expect(result).ToBe(4);
                });

                It("should call through as configured.", () =>
                {
                    Jaz.SpyOn(typeof(ExternSpec), nameof(GetInt)).And.CallThrough();
                    var result = GetInt();
                    Expect(result).ToBe(50);
                });

                It("should throw an exception as configured.", () =>
                {
                    var exception = new TestException();
                    Jaz.SpyOn(typeof(ExternSpec), nameof(GetInt)).And.Throw(exception);
                    var thrown = Expect(() => GetInt()).ToThrow<TestException>();
                    Expect(thrown).ToBe(exception);
                });
            });
        }

        private class TestException : Exception
        {
        }
    }
}
