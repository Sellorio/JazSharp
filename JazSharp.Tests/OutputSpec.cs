using System.Text;
using System.Threading.Tasks;

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

                It("should show output info correctly.", output =>
                {
                    output.AppendLine("This is some friendly text!");
                });

                It("should show output info correctly.", output =>
                {
                    output.AppendLine("This is some friendly text!");
                    return Task.CompletedTask;
                });
            });
        }
    }
}
