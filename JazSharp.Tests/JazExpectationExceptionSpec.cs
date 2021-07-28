namespace JazSharp.Tests
{
    public class JazExpectationExceptionSpec : Spec
    {
        public JazExpectationExceptionSpec()
        {
            Describe<JazExpectationException>(() =>
            {
                It("should trim the stack trace to the test method.", () =>
                {
                    try
                    {
                        Expect(true).ToBeFalse();
                    }
                    catch (JazExpectationException ex)
                    {
                        Expect(ex.StackTrace).ToBe(
                            @"   at JazSharp.Tests.JazExpectationExceptionSpec.<>c.<.ctor>b__0_1() in C:\Users\seamillo\source\repos\JazSharp\JazSharp.Tests\JazExpectationExceptionSpec.cs:line 15");
                        return;
                    }

                    Fail();
                });
            });
        }
    }
}
