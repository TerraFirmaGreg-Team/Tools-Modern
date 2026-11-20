using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
    /// <summary>
    /// A .NET Abstract Class representing a Patchouli Page
    /// </summary>
    [JsonDerivedType(typeof(CraftingPage))]
    [JsonDerivedType(typeof(EmptyPage))]
    [JsonDerivedType(typeof(EntityPage))]
    [JsonDerivedType(typeof(ImagePage))]
    [JsonDerivedType(typeof(LinkPage))]
    [JsonDerivedType(typeof(MultiblockPage))]
    [JsonDerivedType(typeof(QuestPage))]
    [JsonDerivedType(typeof(RelationsPage))]
    [JsonDerivedType(typeof(SmeltingPage))]
    [JsonDerivedType(typeof(SpotlightPage))]
    [JsonDerivedType(typeof(TextPage))]
    public abstract class PatchouliPage
    {
        [JsonPropertyName("type")]
        public abstract string Type { get; }

        [JsonPropertyName("advancement")]
        public string? Advancement { get; set; }

        [JsonPropertyName("anchor")]
        public string? Anchor { get; set; }
    }
}
