using System.Collections;

namespace GSPTestProject.GameData
{
    public class ExampleRules : IEnumerable<object[]>
    {
        public readonly string ExampleDirectory = Path.GetFullPath(Path.Join(Directory.GetCurrentDirectory(), "../../../../Examples/"));

        public IEnumerator<object[]> GetEnumerator ()
        {
            if (!Directory.Exists(ExampleDirectory))
                throw new DirectoryNotFoundException($"Examples directory not found: {ExampleDirectory}");

            foreach (string exampleFile in Directory.GetFiles(ExampleDirectory, "*.json", SearchOption.TopDirectoryOnly))
            {
                yield return new object[] { File.ReadAllText(exampleFile) };
            }
        }

        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
    }
}