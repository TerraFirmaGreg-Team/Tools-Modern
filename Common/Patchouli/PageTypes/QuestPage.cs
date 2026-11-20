using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
    /// <summary>
    /// A .NET Representation of a Patchouli Page for displaying Patchouli Specific Quests
    /// <para></para>
    /// <a href="https://vazkiimods.github.io/Patchouli/docs/patchouli-basics/page-types/#quest-pages-patchouliquest">Patchouli Docs</a>
    /// </summary>
    public class QuestPage : PatchouliPage
    {
        public override string Type => "patchouli:quest";

        [JsonPropertyName("trigger")]
        public required string Trigger { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("text")]
        public required string Text { get; set; }
    }
}
