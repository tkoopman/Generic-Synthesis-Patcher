using System.Collections;

using Common;

using GenericSynthesisPatcher.Rules.Loaders.KID;

using Mutagen.Bethesda.Skyrim;

namespace GSPTestProject.StaticData
{
    public class Data_KidIniContents : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator ()
        {
            yield return new object[] {
                """
                Keyword = OCF_EquipSlot30|Armor|NONE|30
                """,
                new KidIniFile([
                    new KidIniLine("Keyword = OCF_EquipSlot30|Armor|NONE|30", false, new RecordID("OCF_EquipSlot30"), Armor.StaticRegistration, [], ["30"], 100),
                    ])
            };
            yield return new object[] {
                """
                Keyword = KEYWORD1|Armor|0x000001~[A] Mod File.esp
                """,
                new KidIniFile([
                    new KidIniLine("Keyword = KEYWORD1|Armor|0x000001~[A] Mod File.esp", false, new RecordID("KEYWORD1"), Armor.StaticRegistration, ["0x000001~[A] Mod File.esp"], [], 100),
                    ])
            };

            yield return new object[] {
                """
                Keyword = KEYWORD1|Armor|0x000002~[A] Mod File.esp||50
                """,
                new KidIniFile([
                    new KidIniLine("Keyword = KEYWORD1|Armor|0x000002~[A] Mod File.esp||50", false, new RecordID("KEYWORD1"), Armor.StaticRegistration,["0x000002~[A] Mod File.esp"],[], 50),
                    ])
            };

            yield return new object[] {
                """

                ; A random comment

                Keyword = KEYWORD1|Armor|0x000001~[A] Mod File.esp
                Keyword = KEYWORD1|Armor|0x000002~[A] Mod File.esp||50

                """,
                new KidIniFile([
                    new KidIniLine(string.Empty, false, default, null, null, null, 100),
                    new KidIniLine("; A random comment", false, default, null, null, null, 100),
                    new KidIniLine(string.Empty, false, default, null, null, null, 100),
                    new KidIniLine("Keyword = KEYWORD1|Armor|0x000001~[A] Mod File.esp", false, new RecordID("KEYWORD1"), Armor.StaticRegistration,["0x000001~[A] Mod File.esp"],[], 100),
                    new KidIniLine("Keyword = KEYWORD1|Armor|0x000002~[A] Mod File.esp||50", false, new RecordID("KEYWORD1"), Armor.StaticRegistration,["0x000002~[A] Mod File.esp"],[], 50),

                    // Final empty line won't be returned as not returned when looping StringReader.ReadLine()
                    ])
            };

            yield return new object[] {
                """

                ; A random comment

                ; Handled by GSP - Keyword = KEYWORD1|Armor|0x000001~[A] Mod File.esp
                Keyword = KEYWORD1|Armor|0x000002~[A] Mod File.esp||50

                """,
                new KidIniFile([
                    new KidIniLine(string.Empty, false, default, null, null, null, 100),
                    new KidIniLine("; A random comment", false, default, null, null, null, 100),
                    new KidIniLine(string.Empty, false, default, null, null, null, 100),
                    new KidIniLine("Keyword = KEYWORD1|Armor|0x000001~[A] Mod File.esp", true, new RecordID("KEYWORD1"), Armor.StaticRegistration,["0x000001~[A] Mod File.esp"],[], 100),
                    new KidIniLine("Keyword = KEYWORD1|Armor|0x000002~[A] Mod File.esp||50", false, new RecordID("KEYWORD1"), Armor.StaticRegistration,["0x000002~[A] Mod File.esp"],[], 50),

                    // Final empty line won't be returned as not returned when looping StringReader.ReadLine()
                    ])
            };
        }

        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
    }
}