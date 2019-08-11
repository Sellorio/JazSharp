namespace JazSharp.Tests
{
    public class OutputSpec : Spec
    {
        public OutputSpec()
        {
            Describe("Output", () =>
            {
                xIt("should show exception info correctly.", () =>
                {
                    Expect(true).ToBe(false);
                });

                It("should show output info correctly.", () =>
                {
                    Jaz.CurrentTest.Output.AppendLine("This is some friendly text!");
                });
            });
        }
    }
}
