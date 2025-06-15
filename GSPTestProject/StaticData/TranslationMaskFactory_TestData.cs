using System.Collections;

using GenericSynthesisPatcher.Helpers;

namespace GSPTestProject.StaticData
{
    internal class TranslationMaskFactory_TestData : IEnumerable<object?[]>
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
        }

        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
    }
}