using System.Collections;

namespace GSPTestProject.StaticData
{
    internal class TranslationMaskConverter_TestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator ()
        {
            // DefaultOn should be true if no properties are defined
            yield return new object[]
            {
                """
                {
                }
                """,
                new Mutagen.Bethesda.Skyrim.Weapon.TranslationMask(true)
            };

            // DefaultOn should be false as only true entries, even though that property is not a
            // valid property.
            yield return new object[]
            {
                """
                {
                    "something": true
                }
                """,
                new Mutagen.Bethesda.Skyrim.Weapon.TranslationMask(false)
            };

            // DefaultOn should be true as only false entries, even though that property is not a
            // valid property.
            yield return new object[]
            {
                """
                {
                    "something": false
                }
                """,
                new Mutagen.Bethesda.Skyrim.Weapon.TranslationMask(true)
            };

            // Basic boolean value
            yield return new object[]
            {
                "true",
                (Mutagen.Bethesda.Skyrim.Weapon.TranslationMask)true
            };

            // Basic boolean value
            yield return new object[]
            {
                "false",
                (Mutagen.Bethesda.Skyrim.Weapon.TranslationMask)false
            };

            yield return new object[]
            {
                """
                {
                    "defaultOn": true
                }
                """,
                new Mutagen.Bethesda.Skyrim.Weapon.TranslationMask(true)
            };

            yield return new object[]
            {
                """
                {
                    "defaultOn": false
                }
                """,
                new Mutagen.Bethesda.Skyrim.Weapon.TranslationMask(false)
            };

            yield return new object[]
            {
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

            // DefaultOn should be false as at least 1 value not False. Just because it would equate
            // to false it still defined as non False value
            yield return new object[]
            {
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

            // DefaultOn should be true as all defined properties are false. Even though
            // ObjectBounds could be defined as a TranslationMask object.
            yield return new object[]
            {
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
        }

        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
    }
}