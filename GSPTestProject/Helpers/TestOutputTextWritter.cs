using System.Text;

using Xunit.Abstractions;

namespace GSPTestProject.Helpers
{
    public class TestOutputTextWritter (ITestOutputHelper output) : TextWriter
    {
        private readonly ITestOutputHelper _output = output;

        public override Encoding Encoding => Encoding.UTF8;

        public override void Write (string? value) => throw new NotSupportedException("Use WriteLine instead of Write");

        public override void WriteLine (string? value)
        {
            if (value is not null)
                _output.WriteLine(value);
        }

        public override void WriteLine () => _output.WriteLine(string.Empty);
    }
}