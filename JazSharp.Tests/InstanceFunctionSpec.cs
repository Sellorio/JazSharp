using System;

namespace JazSharp.Tests
{
    public class InstanceFunctionSpec : Spec
    {
        public InstanceFunctionSpec()
        {
            Describe("An Instance Function", () =>
            {
                var subject = new TestSubject();

                It("should call through by default.", () =>
                {
                    var result = subject.Add5(3);
                    Expect(result).ToBe(8);
                });

                It("should do nothing if spied on.", () =>
                {
                    Jaz.SpyOn(subject, nameof(subject.Add5));
                    var result = subject.Add5(3);
                    Expect(result).ToBeDefault();
                });

                It("should return the configured value.", () =>
                {
                    Jaz.SpyOn(subject, nameof(subject.Add5)).And.ReturnValue(1);
                    var result = subject.Add5(3);
                    Expect(result).ToBe(1);
                });

                It("should return the configured values in sequence.", () =>
                {
                    Jaz.SpyOn(subject, nameof(subject.Add5)).And.ReturnValues(1, 2, 3, 4);
                    var result = subject.Add5(3);
                    Expect(result).ToBe(1);
                    result = subject.Add5(3);
                    Expect(result).ToBe(2);
                    result = subject.Add5(3);
                    Expect(result).ToBe(3);
                    result = subject.Add5(3);
                    Expect(result).ToBe(4);
                });

                It("should call through as configured.", () =>
                {
                    Jaz.SpyOn(subject, nameof(subject.Add5)).And.CallThrough();
                    var result = subject.Add5(3);
                    Expect(result).ToBe(8);
                });

                It("should throw an exception as configured.", () =>
                {
                    var exception = new TestException();
                    Jaz.SpyOn(subject, nameof(subject.Add5)).And.Throw(exception);
                    var thrown = Expect(() => subject.Add5(3)).ToThrow<TestException>();
                    Expect(thrown).ToBe(exception);
                });
            });
        }

        private class TestSubject
        {
            public int Add5(int value)
            {
                return value + 5;
            }
        }

        private class TestException : Exception
        {
        }
    }
}
