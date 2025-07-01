using System.Collections;

using GenericSynthesisPatcher.Helpers;

using Loqui;

using Mutagen.Bethesda.Plugins.Records;

namespace GSPTestProject.StaticData
{
    internal class TranslationMaskFactory_TestData
    {
        internal class SetValueAdvanced_TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator ()
            {
                yield return new object[]
                {
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
                    new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false),
                    new Dictionary<string, (object?, bool)>
                    { // Shouldn't allow setting bool value to null
                        {"EditorID", ((ITranslationMask?)null, false)},
                    },
                    new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false)
                };

                yield return new object[]
                {
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
                    new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false),
                    new Dictionary<string, (object?, bool)>
                    { // Incorrect ITranslationMask type
                        {"BodyTemplate", (new Mutagen.Bethesda.Skyrim.Cell.TranslationMask(true), false)},
                    },
                    new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false)
                };

                yield return new object[]
                {
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
                    new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false),
                    new Dictionary<string, (object?, bool)>
                    { // Incorrect ITranslationMask type
                        {"WorldModel", (new GenderedItem<Mutagen.Bethesda.Skyrim.Armor.TranslationMask>(new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(true, false), new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false, true)), false)},
                    },
                    new Mutagen.Bethesda.Skyrim.Armor.TranslationMask(false)
                };

                yield return new object[]
                {
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
                // Incorrect Type
                yield return new object?[]
                {
                typeof(Mutagen.Bethesda.Skyrim.Weapon),
                true,
                null,
                (IEnumerable<string>)[],
                StringComparison.Ordinal,
                false,
                null,
                };

                // Basic true
                yield return new object?[]
                {
                typeof(Mutagen.Bethesda.Skyrim.Weapon.TranslationMask),
                true,
                null,
                (IEnumerable<string>)[],
                StringComparison.Ordinal,
                true,
                new Mutagen.Bethesda.Skyrim.Weapon.TranslationMask(true),
                };

                // Basic false
                yield return new object?[]
                {
                typeof(Mutagen.Bethesda.Skyrim.Weapon.TranslationMask),
                false,
                null,
                (IEnumerable<string>)[],
                StringComparison.Ordinal,
                true,
                new Mutagen.Bethesda.Skyrim.Weapon.TranslationMask(false),
                };

                // Fails but returns mask with model not set as incorrect case
                yield return new object?[]
                {
                typeof(Mutagen.Bethesda.Skyrim.Weapon.TranslationMask),
                true,
                null,
                (IEnumerable<string>)["Name", "model"],
                StringComparison.Ordinal,
                false,
                new Mutagen.Bethesda.Skyrim.Weapon.TranslationMask(true) {Name = false},
                };

                // Success as case insensitive
                yield return new object?[]
                {
                typeof(Mutagen.Bethesda.Skyrim.Weapon.TranslationMask),
                true,
                null,
                (IEnumerable<string>)["Name", "model"],
                StringComparison.OrdinalIgnoreCase,
                true,
                new Mutagen.Bethesda.Skyrim.Weapon.TranslationMask(true) {Name = false, Model = false},
                };

                // Fails but returns mask with model not set as incorrect case
                yield return new object?[]
                {
                typeof(Mutagen.Bethesda.Skyrim.Weapon.TranslationMask),
                true,
                false,
                (IEnumerable<string>)["Name"],
                StringComparison.Ordinal,
                true,
                new Mutagen.Bethesda.Skyrim.Weapon.TranslationMask(true, false) {Name = false},
                };

                // Fails but returns mask with model not set as incorrect case
                yield return new object?[]
                {
                typeof(Mutagen.Bethesda.Skyrim.Weapon.TranslationMask),
                true,
                true,
                (IEnumerable<string>)[],
                StringComparison.Ordinal,
                true,
                (Mutagen.Bethesda.Skyrim.Weapon.TranslationMask)true,
                };

                // Incorrect Type
                Assert.True(TranslationMaskFactory.TryGetTranslationMaskType(Mutagen.Bethesda.Skyrim.Npc.StaticRegistration, out var mask));
                yield return new object?[]
                {
                mask,
                false,
                null,
                (IEnumerable<string>)["VirtualMachineAdapter"],
                StringComparison.Ordinal,
                true,
                new Mutagen.Bethesda.Skyrim.Npc.TranslationMask(false) {VirtualMachineAdapter = new(true, true)},
                };

                // Success using alias
                yield return new object?[]
                {
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