using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
    /// <summary>
    /// A .NET Representation of a Patchouli Page for displaying Text
    /// <para></para>
    /// <a href="https://vazkiimods.github.io/Patchouli/docs/patchouli-basics/page-types/#text-pages-patchoulitext">Patchouli Docs</a>
    /// </summary>
    public class TextPage : PatchouliPage
    {
        public override string Type => "patchouli:text";

        [JsonPropertyName("text")]
        public required string Text { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }
    }
}
