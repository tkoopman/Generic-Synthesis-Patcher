using Common;

using GenericSynthesisPatcher.Rules.Loaders.KID;

using GSPTestProject.GameData.GlobalGame.Fixtures;
using GSPTestProject.Helpers;
using GSPTestProject.StaticData;

using Xunit.Abstractions;

namespace GSPTestProject
{
    [Collection("Sequential")]
    public class KIDTests_Skyrim (SkyrimSEFixture skyrimFixture, ITestOutputHelper output) : IClassFixture<SkyrimSEFixture>
    {
        public static readonly char[] ValidChars = [.. "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz"];
        private readonly TextWriter _writer = new TestOutputTextWritter(output);
        private readonly ITestOutputHelper Output = output;

        [Theory]
        [ClassData(typeof(Data_KidIniContents))]
        public void Test_KIDFile (string fileContents, KidIniFile expected)
        {
            var actual = new KidIniFile(fileContents);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_LoadExternalKIDData ()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Assert.True(KidLoader.TryLoadRules(0, @"E:\Temp\KIDData\", out var rules));

            watch.Stop();
            Output.WriteLine($"Load Rules: {watch.Elapsed}");
            watch.Restart();

            HashSet<string> toFindWildCards = [];

            var index = new IndexedRecordIDs<KidRule>();

            foreach (var rule in rules)
            {
                if (rule is KidRule kidRule)
                {
                    foreach (var id in kidRule.AllStrings)
                    {
                        if (id.Type is IDType.FormKey or IDType.ModKey or IDType.Name)
                        {
                            index.Add(id, kidRule);
                            if (id.Type is IDType.Name && id.IsWildcard)
                                _ = toFindWildCards.Add(id.Name);
                        }
                        else
                        {
                            Output.WriteLine(id.ToString());
                        }
                    }

                    foreach (var id in kidRule.AnyStrings)
                    {
                        if (id.Type is IDType.FormKey or IDType.ModKey or IDType.Name)
                        {
                            index.Add(id, kidRule);
                            if (id.Type is IDType.Name && id.IsWildcard)
                                _ = toFindWildCards.Add(id.Name);
                        }
                        else
                        {
                            Output.WriteLine(id.ToString());
                        }
                    }

                    foreach (var id in kidRule.NotStrings)
                    {
                        if (id.Type is IDType.FormKey or IDType.ModKey or IDType.Name)
                        {
                            index.Add(id, kidRule);
                            if (id.Type is IDType.Name && id.IsWildcard)
                                _ = toFindWildCards.Add(id.Name);
                        }
                        else
                        {
                            Output.WriteLine(id.ToString());
                        }
                    }
                }
            }

            watch.Stop();
            Output.WriteLine($"Load Rules into Index: {watch.Elapsed}");
            watch.Restart();

            index.SortIndex();

            watch.Stop();
            Output.WriteLine($"Sort Index: {watch.Elapsed}");

            index.PrintStats(_writer);

            List<string> random = [];
            Random rnd = new();

            for (int i = 0; i < 100; i++)
            {
                int len = rnd.Next(4, 16);
                char[] chars = new char[len];
                for (int l = 0; l < len; l++)
                    chars[l] = ValidChars[rnd.Next(ValidChars.Length)];

                string str = string.Join(null, chars);
                if (!toFindWildCards.Any(w => str.Contains(w, StringComparison.OrdinalIgnoreCase)))
                    random.Add(str);
            }

            watch.Restart();
            foreach (string w in toFindWildCards)
            {
                int count = index.InternalFindAll(w, RecordID.Field.EditorID).Count();
                Assert.True(count > 0);

                for (int i = 0; i < 1000; i++)
                {
                    string r = random[rnd.Next(random.Count)];
                    count = index.InternalFindAll($"{w}{r}", RecordID.Field.EditorID).Count();
                    Assert.True(count > 0);
                    count = index.InternalFindAll($"{r}{w}", RecordID.Field.EditorID).Count();
                    Assert.True(count > 0);
                }
            }

            watch.Stop();
            Output.WriteLine($"Searches for {toFindWildCards.Count}k took {watch.Elapsed}");
        }
    }
}