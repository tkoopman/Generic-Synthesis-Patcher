using System.ComponentModel;

using GenericSynthesisPatcher.Games.Universal.Json.Action;
using GenericSynthesisPatcher.Games.Universal.Json.Operations;

using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Games.Skyrim.Json.Action
{
    public class RelationsData (FormKeyListOperation<IRelatableGetter> formKey, int modifier, CombatReaction reaction) : FormLinksWithDataActionDataBase<IRelatableGetter, Relation>, IEquatable<RelationsData>
    {
        [JsonProperty(PropertyName = "Target", Required = Required.Always)]
        public override FormKeyListOperation<IRelatableGetter> FormKey { get; } = formKey ?? new(null);

        [JsonProperty(PropertyName = "Modifier", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public int Modifier { get; set; } = modifier;

        [JsonProperty(PropertyName = "Reaction", Required = Required.Always)]
        public CombatReaction Reaction { get; set; } = reaction;

        public static bool Equals (IFormLinkContainerGetter left, IFormLinkContainerGetter right)
            => left is IRelationGetter l
            && right is IRelationGetter r
            && l.Target.FormKey.Equals(r.Target.FormKey)
            && l.Modifier == r.Modifier
            && l.Reaction == r.Reaction;

        public bool Equals (RelationsData? other) => FormKey.Equals(other?.FormKey) && Modifier == other?.Modifier && Reaction == other?.Reaction;

        public override bool Equals (object? obj) => Equals(obj as RelationsData);

        public override bool Equals (IFormLinkContainerGetter? other)
            => other is IRelationGetter otherRelation
            && FormKey.ValueEquals(otherRelation.Target.FormKey)
            && otherRelation.Modifier == Modifier
            && otherRelation.Reaction == Reaction;

        public override int GetHashCode () => FormKey.GetHashCode() ^ Modifier ^ (int)Reaction;

        public override Relation ToActionData ()
        {
            var relation = new Relation();
            relation.Target.FormKey = FormKey.Value;
            relation.Modifier = Modifier;
            relation.Reaction = Reaction;

            return relation;
        }

        public override string? ToString () => $"{Reaction}-{FormKey.Value}";
    }
}