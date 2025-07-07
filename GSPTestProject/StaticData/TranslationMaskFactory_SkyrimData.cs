using System.Collections;

using GenericSynthesisPatcher.Helpers;

using Loqui;

using Mutagen.Bethesda.Plugins.Records;

namespace GSPTestProject.StaticData
{
    internal class TranslationMaskFactory_SkyrimData
    {
        internal class SetValueAdvanced_TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator ()
            {
                yield return new object[]
                {
                    "Simple test 1",
                    new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false),
                    new Dictionary<string, (object?, bool)>
                    {
                        {"EditorID", (true, true)},
                    },
                    new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false)
                    {
                        EditorID = true,
                    }
                };

                yield return new object[]
                {
                    "Checks doesn't error on trying to set bool value to null",
                    new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false),
                    new Dictionary<string, (object?, bool)>
                    {
                        {"EditorID", ((ITranslationMask?)null, false)},
                    },
                    new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false)
                };

                yield return new object[]
                {
                    "Checks can set ITranslationMask field using bool",
                    new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false),
                    new Dictionary<string, (object?, bool)>
                    {
                        {"BodyTemplate", (true, true)},
                    },
                    new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false)
                    {
                        BodyTemplate = true,
                    }
                };

                yield return new object[]
                {
                    "Checks setting ITranslationMask field to valid object",
                    new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false),
                    new Dictionary<string, (object?, bool)>
                    {
                        {"BodyTemplate", (new Mutagen.Bethesda.Skyrim.BodyTemplate.TranslationMask(true, false), true)},
                    },
                    new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false)
                    {
                        BodyTemplate = new(true, false),
                    }
                };

                yield return new object[]
                {
                    "Checks setting ITranslationMask object to incorrect object type",
                    new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false),
                    new Dictionary<string, (object?, bool)>
                    {
                        {"BodyTemplate", (new Mutagen.Bethesda.Skyrim.Cell.TranslationMask(true), false)},
                    },
                    new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false)
                };

                yield return new object[]
                {
                    "Checks setting ITranslationMask field to null",
                    new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false)
                    {
                        BodyTemplate = true,
                    },
                    new Dictionary<string, (object?, bool)>
                    {
                        {"BodyTemplate", ((ITranslationMask?)null, true)},
                    },
                    new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false)
                };

                yield return new object[]
                {
                    "Tests setting Gendered ITranslationMask object using single bool (Assign to both genders)",
                    new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false),
                    new Dictionary<string, (object?, bool)>
                    {
                        {"WorldModel", (true, true)},
                    },
                    new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false)
                    {
                        WorldModel = new(true, true),
                    }
                };

                yield return new object[]
                {
                    "Tests setting Gendered ITranslationMask object using single ITranslationMask object (Assign to both genders)",
                    new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false),
                    new Dictionary<string, (object?, bool)>
                    {
                        {"WorldModel", (new Mutagen.Bethesda.Skyrim.ArmorModel.TranslationMask(true, false), true)},
                    },
                    new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false)
                    {
                        WorldModel = new(new Mutagen.Bethesda.Skyrim.ArmorModel.TranslationMask(true, false), new Mutagen.Bethesda.Skyrim.ArmorModel.TranslationMask(true, false)),
                    }
                };

                yield return new object[]
                {
                    "Tests setting Gendered ITranslationMask object using valid gender object",
                    new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false),
                    new Dictionary<string, (object?, bool)>
                    {
                        {"WorldModel", (new GenderedItem<Mutagen.Bethesda.Skyrim.ArmorModel.TranslationMask>(new Mutagen.Bethesda.Skyrim.ArmorModel.TranslationMask(true, false), new Mutagen.Bethesda.Skyrim.ArmorModel.TranslationMask(false, true)), true)},
                    },
                    new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false)
                    {
                        WorldModel = new(new Mutagen.Bethesda.Skyrim.ArmorModel.TranslationMask(true, false), new Mutagen.Bethesda.Skyrim.ArmorModel.TranslationMask(false, true)),
                    }
                };

                yield return new object[]
                {
                    "Tests setting Gendered ITranslationMask object using gendered object with incorrect ITranslationMask type",
                    new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false),
                    new Dictionary<string, (object?, bool)>
                    {
                        {"WorldModel", (new GenderedItem<Mutagen.Bethesda.Skyrim.Armor.TranslationMask>(new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(true, false), new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false, true)), false)},
                    },
                    new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false)
                };

                yield return new object[]
                {
                    "Tests setting Gendered ITranslationMask object to null",
                    new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false)
                    {
                        WorldModel = new(true, true),
                    },
                    new Dictionary<string, (object?, bool)>
                    {
                        {"WorldModel", ((ITranslationMask?)null, true)},
                    },
                    new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false)
                };
            }

            IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
        }

        internal class TryCreate_TestData : IEnumerable<object?[]>
        {
            public IEnumerator<object?[]> GetEnumerator ()
            {
                yield return new object?[]
                {
                    "Fail as type is an ITranslationMask",
                    typeof(Mutagen.Bethesda.Skyrim.Weapon),
                    true,
                    null,
                    (IEnumerable<string>)[],
                    StringComparison.Ordinal,
                    false,
                    null,
                };

                yield return new object?[]
                {
                    "Basic bool (true) test",
                    typeof(Mutagen.Bethesda.Skyrim.Weapon.TranslationMask),
                    true,
                    null,
                    (IEnumerable<string>)[],
                    StringComparison.Ordinal,
                    true,
                    new Mutagen.Bethesda.Skyrim.Weapon.TranslationMask(true),
                };

                yield return new object?[]
                {
                    "Basic bool (false) test",
                    typeof(Mutagen.Bethesda.Skyrim.Weapon.TranslationMask),
                    false,
                    null,
                    (IEnumerable<string>)[],
                    StringComparison.Ordinal,
                    true,
                    new Mutagen.Bethesda.Skyrim.Weapon.TranslationMask(false),
                };

                yield return new object?[]
                {
                    "Partial success. Fail to toggle model as won't exact match",
                    typeof(Mutagen.Bethesda.Skyrim.Weapon.TranslationMask),
                    true,
                    null,
                    (IEnumerable<string>)["Name", "model"],
                    StringComparison.Ordinal,
                    false,
                    new Mutagen.Bethesda.Skyrim.Weapon.TranslationMask(true) {Name = false},
                };

                yield return new object?[]
                {
                    "Basic test with case insensitive match toggle",
                    typeof(Mutagen.Bethesda.Skyrim.Weapon.TranslationMask),
                    true,
                    null,
                    (IEnumerable<string>)["Name", "model"],
                    StringComparison.OrdinalIgnoreCase,
                    true,
                    new Mutagen.Bethesda.Skyrim.Weapon.TranslationMask(true) {Name = false, Model = false},
                };

                yield return new object?[]
                {
                    "Basic test with exact match toggle",
                    typeof(Mutagen.Bethesda.Skyrim.Weapon.TranslationMask),
                    true,
                    false,
                    (IEnumerable<string>)["Name"],
                    StringComparison.Ordinal,
                    true,
                    new Mutagen.Bethesda.Skyrim.Weapon.TranslationMask(true, false) {Name = false},
                };

                yield return new object?[]
                {
                    "Basic bool test",
                    typeof(Mutagen.Bethesda.Skyrim.Weapon.TranslationMask),
                    true,
                    true,
                    (IEnumerable<string>)[],
                    StringComparison.Ordinal,
                    true,
                    (Mutagen.Bethesda.Skyrim.Weapon.TranslationMask)true,
                };

                Assert.True(TranslationMaskFactory.TryGetTranslationMaskType(Mutagen.Bethesda.Skyrim.Npc.StaticRegistration, out var mask));
                yield return new object?[]
                {
                    "Toggle ITranslatedMask field type",
                    mask,
                    false,
                    null,
                    (IEnumerable<string>)["VirtualMachineAdapter"],
                    StringComparison.Ordinal,
                    true,
                    new Mutagen.Bethesda.Skyrim.Npc.TranslationMask(false) {VirtualMachineAdapter = new(true, true)},
                };

                yield return new object?[]
                {
                    "Toggle using alias",
                    typeof(Mutagen.Bethesda.Skyrim.Weapon.TranslationMask),
                    true,
                    null,
                    (IEnumerable<string>)["full", "model"],
                    StringComparison.OrdinalIgnoreCase,
                    true,
                    new Mutagen.Bethesda.Skyrim.Weapon.TranslationMask(true) {Name = false, Model = false},
                };
            }

            IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
        }
    }
}