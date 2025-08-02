using System.Collections;

namespace GSPTestProject.GameData
{
    public class SkyrimExamples : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator ()
        {
            string ExampleDirectory = Path.GetFullPath(Path.Join(Directory.GetCurrentDirectory(), "../../../../Examples/"));

            foreach (string file in Directory.GetFiles(ExampleDirectory, "*.json", SearchOption.TopDirectoryOnly))
            {
                using var jsonFile = File.OpenText(file);
                string contents = jsonFile.ReadToEnd();

                yield return new object[] { contents };
            }
        }

        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
    }
}