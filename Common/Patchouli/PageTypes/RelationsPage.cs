using System.Text.Json.Serialization;

namespace Common.Patchouli
{
    /// <summary>
    /// A .NET Representation of a Patchouli Page for displaying Relations
    /// <para></para>
    /// <a href="https://vazkiimods.github.io/Patchouli/docs/patchouli-basics/page-types/#relations-pages-patchoulirelations">Patchouli Docs</a>
    /// </summary>
    public class RelationsPage : PatchouliPage
    {
        public override string Type => "patchouli:relations";

        [JsonPropertyName("entries")]
        public required string[] Entries { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
}
