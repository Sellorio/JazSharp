namespace JazSharp.Tests
{
    public class BaseCallSpec : Spec
    {
        public BaseCallSpec()
        {
            Describe("Calls to base.XXX", () =>
            {
                It("calls through without issues.", () =>
                {
                    var testSubject = new TestSubject();
                    Expect(testSubject.GetInt()).ToBe(8);
                });
            });
        }

        public class TestSubjectBase
        {
            public virtual int GetInt()
            {
                return 5;
            }
        }

        public class TestSubject : TestSubjectBase
        {
            public override int GetInt()
            {
                return base.GetInt() + 3;
            }
        }
    }
}
