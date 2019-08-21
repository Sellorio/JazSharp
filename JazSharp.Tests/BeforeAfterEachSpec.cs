namespace JazSharp.Tests
{
    public class BeforeAfterEachSpec : Spec
    {
        private string _orderedKey = string.Empty;

        public BeforeAfterEachSpec()
        {
            Describe("Before and after each", () =>
            {
                BeforeEach(output =>
                {
                    _orderedKey += "A";
                    output.AppendLine("A");
                });

                BeforeEach(output =>
                {
                    _orderedKey += "B";
                    output.AppendLine("B");
                });

                Describe("when scoped", () =>
                {
                    BeforeEach(output =>
                    {
                        _orderedKey += "C";
                        output.AppendLine("C");
                    });

                    It("should run out-in and in-out respectively.", () =>
                    {
                    });

                    AfterEach(output =>
                    {
                        _orderedKey += "C";
                        output.AppendLine("C");
                    });
                });

                Describe("(no tests, not run)", () =>
                {
                    BeforeEach(output =>
                    {
                        _orderedKey += "E";
                        output.AppendLine("E");
                    });

                    AfterEach(output =>
                    {
                        _orderedKey += "E";
                        output.AppendLine("E");
                    });
                });

                AfterEach(output =>
                {
                    _orderedKey += "B";
                    output.AppendLine("B");
                });

                AfterEach(output =>
                {
                    _orderedKey += "A";
                    output.AppendLine("A");
                    Expect(_orderedKey).ToBe("ABCCBA");
                });
            });
        }
    }
}
