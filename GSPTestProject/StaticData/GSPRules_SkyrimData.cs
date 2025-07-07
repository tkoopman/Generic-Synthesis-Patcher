using System.Collections;

using GenericSynthesisPatcher.Games.Universal.Json.Data;

using Mutagen.Bethesda.Skyrim;

namespace GSPTestProject.StaticData
{
    public class GSPRules_SkyrimData : IEnumerable<object?[]>
    {
        public IEnumerator<object?[]> GetEnumerator ()
        {
            // As many options as possible, while still being valid (some just ignored), with some
            // deprecated fields
            yield return new object?[]
            {
                """
                [
                    {
                        // GSPBase
                        "Debug": true,
                        "Patched": false,
                        "Priority": 1000,
                        "types": "NPC",
                        "Masters": ["Skyrim.esm"],
                        "PatchedBy": ["SkyrimSEPatch.esp"],

                        // GSPRule
                        "EditorID": "Blah",
                        "FormID": "800:Skyrim.esm",

                        // Deprecated
                        "ForwardIndexedByField": true,
                        "ForwardType": "SelfMasterOnly",

                        // GSPGroup Ignored
                        "SingleMatch": true,

                        // Actions
                        "Fill": {"Property": "Value"},
                    },
                ]
                """,
                true, // expectedValidate
                new Func<List<GSPBase>, bool> ((rules)
                    => rules.Count == 1
                    && rules.First() is GSPRule rule
                    && rule.Debug == true
                    && rule.Patched == false
                    && rule.Priority == 1000
                    && rule.Types.Count() == 1
                    && rule.Types.First() == INpc.StaticRegistration
                    && rule.Masters?.Count == 1
                    && rule.PatchedBy?.Count == 1
                    && rule.EditorID?.Count == 1
                    && rule.FormID?.Count == 1
                    && rule.ForwardOptions == (ForwardOptions.IndexedByField | ForwardOptions.SelfMasterOnly)
                    && rule.Fill?.Count == 1
                    )
            };

            // Invalid no types
            yield return new object?[]
            {
                """
                [
                    {
                        // GSPBase
                        "Priority": 1000,
                        "Masters": ["Skyrim.esm"],
                        "PatchedBy": ["SkyrimSEPatch.esp"],

                        // GSPRule
                        "EditorID": "Blah",

                        // Actions
                        "Fill": {"Property": "Value"},
                    },
                ]
                """,
                false, // expectedValidate
                null // customValidate
            };

            // DeepCopyIn
            yield return new object?[]
            {
                """"
                [
                    {
                        // GSPBase
                        "Types": "NPC",
                        "DeepCopyIn": [
                        {
                            "FormID": "800:Skyrim.esm",
                            "FromMod": "Skyrim.esm",
                            "Mask":
                            {
                                "FaceParts": true,
                                "ObjectBounds": true,
                            }
                        }
                        ]
                    },
                ]
                """",
                true, // expectedValidate
                new Func<List<GSPBase>, bool> ((rules)
                    => rules.Count == 1
                    && rules.First() is GSPRule rule
                    && rule.DeepCopyIn.Count == 1
                    && rule.DeepCopyIn.First().GetMask(typeof(Npc.TranslationMask)) is Npc.TranslationMask mask
                    && mask.DefaultOn == false
                    && mask.ObjectBounds is not null
                    && mask.FaceParts is not null
                    && mask.ObjectBounds.DefaultOn == true
                    && mask.FaceParts.DefaultOn == true
                    )
            };
        }

        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
    }
}