using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
    /// <summary>
    /// A .NET Representation of a Patchouli Page for displaying Images
    /// <para></para>
    /// <a href="https://vazkiimods.github.io/Patchouli/docs/patchouli-basics/page-types/#image-pages-patchouliimage">Patchouli Docs</a>
    /// </summary>
    public class ImagePage : PatchouliPage
    {
        public override string Type => "patchouli:image";

        [JsonPropertyName("images")]
        public required string[] Images { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("border")]
        public bool Border { get; set; }

        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
}
