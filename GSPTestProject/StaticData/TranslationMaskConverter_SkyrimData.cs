using System.Collections;

namespace GSPTestProject.StaticData
{
    internal class TranslationMaskConverter_SkyrimData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator ()
        {
            yield return new object[]
            {
                "Empty Json. DefaultOn should be true if no properties are defined",
                """
                {
                }
                """,
                new Mutagen.Bethesda.Skyrim.Weapon.TranslationMask(true)
            };

            yield return new object[]
            {
                "Only invalid true entries. DefaultOn should be false.",
                """
                {
                    "something": true
                }
                """,
                new Mutagen.Bethesda.Skyrim.Weapon.TranslationMask(false)
            };

            yield return new object[]
            {
                "Only invalid false entries. DefaultOn should be true.",
                """
                {
                    "something": false
                }
                """,
                new Mutagen.Bethesda.Skyrim.Weapon.TranslationMask(true)
            };

            yield return new object[]
            {
                "Basic boolean (true)",
                "true",
                (Mutagen.Bethesda.Skyrim.Weapon.TranslationMask)true
            };

            yield return new object[]
            {
                "Basic boolean (false)",
                "false",
                (Mutagen.Bethesda.Skyrim.Weapon.TranslationMask)false
            };

            yield return new object[]
            {
                "Basic valid Json object 1",
                """
                {
                    "defaultOn": true
                }
                """,
                new Mutagen.Bethesda.Skyrim.Weapon.TranslationMask(true)
            };

            yield return new object[]
            {
                "Basic valid Json object 2",
                """
                {
                    "defaultOn": false
                }
                """,
                new Mutagen.Bethesda.Skyrim.Weapon.TranslationMask(false)
            };

            yield return new object[]
            {
                "Basic valid Json object 3",
                """
                {
                    "defaultOn": true,
                    "OnOverall": true
                }
                """,
                new Mutagen.Bethesda.Skyrim.Weapon.TranslationMask(true)
            };

            yield return new object[]
            {
                "Basic valid Json object 4",
                """
                {
                    "defaultOn": false,
                    "OnOverall": false
                }
                """,
                new Mutagen.Bethesda.Skyrim.Weapon.TranslationMask(false, false)
            };

            yield return new object[]
            {
                "Json object with multiple fields set, including an ITranslationMask field.",
                """
                {
                    "defaultOn": true,
                    "OnOverall": false,
                    "ObjectEffect": false,
                    "FormVersion": false,
                    "ObjectBounds": false,
                    "Model": {
                    "DefaultOn": true,
                    "OnOverall": false
                    }
                }
                """,
                new Mutagen.Bethesda.Skyrim.Weapon.TranslationMask(true, false)
                {
                    ObjectEffect = false,
                    FormVersion = false,
                    ObjectBounds = (Mutagen.Bethesda.Skyrim.ObjectBounds.TranslationMask)false,
                    Model = new Mutagen.Bethesda.Skyrim.Model.TranslationMask(true, false)
                }
            };

            yield return new object[]
            {
                "DefaultOn should be false as at least 1 value not False. Just because it would equate to false it still defined as non False value",
                """
                {
                    "ObjectEffect": false,
                    "FormVersion": false,
                    "ObjectBounds": false,
                    "Model": {
                    "DefaultOn": false
                    }
                }
                """,
                new Mutagen.Bethesda.Skyrim.Weapon.TranslationMask(false, true)
                {
                    ObjectEffect = false,
                    FormVersion = false,
                    ObjectBounds = (Mutagen.Bethesda.Skyrim.ObjectBounds.TranslationMask)false,
                    Model = new Mutagen.Bethesda.Skyrim.Model.TranslationMask(false)
                }
            };

            yield return new object[]
            {
                "DefaultOn should be true as all defined properties are bool false.",
                """
                {
                    "OnOverall": false,
                    "ObjectEffect": false,
                    "FormVersion": false,
                    "ObjectBounds": false,
                }
                """,
                new Mutagen.Bethesda.Skyrim.Weapon.TranslationMask(true, false)
                {
                    ObjectEffect = false,
                    FormVersion = false,
                    ObjectBounds = (Mutagen.Bethesda.Skyrim.ObjectBounds.TranslationMask)false,
                }
            };

            yield return new object[]
            {
                "Test fields of ITranslationMask type being set by bool and Json objects",
                """
                {
                    "defaultOn": true,
                    "OnOverall": false,
                    "ObjectEffect": false,
                    "FormVersion": false,
                    "ObjectBounds": true,
                    "Model": {
                    "DefaultOn": true
                    }
                }
                """,
                new Mutagen.Bethesda.Skyrim.Weapon.TranslationMask(true, false)
                {
                    ObjectEffect = false,
                    FormVersion = false,
                    ObjectBounds = (Mutagen.Bethesda.Skyrim.ObjectBounds.TranslationMask)true,
                    Model = new Mutagen.Bethesda.Skyrim.Model.TranslationMask(true, true)
                }
            };

            yield return new object[]
            {
                "Test gendered ITranslationMask field being set to single bool value.",
                """
                {
                    "defaultOn": false,
                    "WorldModel": true
                }
                """,
                new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false)
                {
                    WorldModel = new(new Mutagen.Bethesda.Skyrim.ArmorModel.TranslationMask(true), new Mutagen.Bethesda.Skyrim.ArmorModel.TranslationMask(true))
                }
            };

            yield return new object[]
            {
                "Test gendered ITranslationMask field being set to single Json object value.",
                """
                {
                    "defaultOn": false,
                    "WorldModel": {
                        "DefaultOn": true,
                        "OnOverall": true
                    }
                }
                """,
                new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false)
                {
                    WorldModel = new(new Mutagen.Bethesda.Skyrim.ArmorModel.TranslationMask(true, true), new Mutagen.Bethesda.Skyrim.ArmorModel.TranslationMask(true, true))
                }
            };

            yield return new object[]
            {
                "Test gendered ITranslationMask field being set with separate male/female Json object values.",
                """
                {
                    "defaultOn": false,
                    "WorldModel": {
                        "Male": {
                            "DefaultOn": false,
                            "OnOverall": true
                        },
                        "Female": {
                            "DefaultOn": true,
                            "OnOverall": false
                        }
                    }
                }
                """,
                new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false)
                {
                    WorldModel = new(new Mutagen.Bethesda.Skyrim.ArmorModel.TranslationMask(false, true), new Mutagen.Bethesda.Skyrim.ArmorModel.TranslationMask(true, false)),
                }
            };

            yield return new object[]
            {
                "Test gendered ITranslationMask field being set with separate male/female bool values.",
                """
                {
                    "defaultOn": false,
                    "Priority": {
                        "Male": false,
                        "Female": true
                    }
                }
                """,
                new Mutagen.Bethesda.Skyrim.ArmorAddon.TranslationMask(false)
                {
                    Priority = new(false, true),
                }
            };

            yield return new object[]
            {
                "Test gendered bool field being set with separate male/female values.",
                """
                {
                    "defaultOn": false,
                    "Priority": true
                }
                """,
                new Mutagen.Bethesda.Skyrim.ArmorAddon.TranslationMask(false)
                {
                    Priority = new(true, true),
                }
            };

            yield return new object[]
            {
                "Test setting field value using alias",
                """
                {
                    "full": true
                }
                """,
                new Mutagen.Bethesda.Skyrim.Npc.TranslationMask(false)
                {
                    Name = true,
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
    }
}