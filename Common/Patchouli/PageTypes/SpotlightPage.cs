using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
    /// <summary>
    /// A .NET Representation of a Patchouli Page for Spotlights
    /// <para></para>
    /// <a href="https://vazkiimods.github.io/Patchouli/docs/patchouli-basics/page-types/#spotlight-pages-patchoulispotlight">Patchouli Docs</a>
    /// </summary>
    public class SpotlightPage : PatchouliPage
    {
        public override string Type => "patchouli:spotlight";

        [JsonPropertyName("item")]
        public required string Item { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("link_recipe")]
        public bool LinkRecipe { get; set; } = false;

        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
}
