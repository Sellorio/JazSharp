using System;

namespace JazSharp.Tests
{
    public class InstanceActionSpec : Spec
    {
        public InstanceActionSpec()
        {
            Describe("An Instance Action", () =>
            {
                var subject = new TestSubject();

                It("should call through by default.", () =>
                {
                    subject.Iterate(3);
                    Expect(subject.Value).ToBe(3);
                });

                It("should do nothing if spied on.", () =>
                {
                    Jaz.SpyOn(subject, nameof(subject.Iterate));
                    subject.Iterate(3);
                    Expect(subject.Value).ToBeDefault();
                });

                It("should call through as configured.", () =>
                {
                    Jaz.SpyOn(subject, nameof(subject.Iterate)).And.CallThrough();
                    subject.Iterate(3);
                    Expect(subject.Value).ToBe(3);
                });

                It("should throw an exception as configured.", () =>
                {
                    var exception = new TestException();
                    Jaz.SpyOn(subject, nameof(subject.Iterate)).And.Throw(exception);
                    var thrown = Expect(() => subject.Iterate(3)).ToThrow<TestException>();
                    Expect(thrown).ToBe(exception);
                });
            });
        }

        private class TestSubject
        {
            public int Value;

            public void Iterate(int amount)
            {
                Value += amount;
            }
        }

        private class TestException : Exception
        {
        }
    }
}
