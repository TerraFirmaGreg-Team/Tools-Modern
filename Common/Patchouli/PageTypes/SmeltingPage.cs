using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
    /// <summary>
    /// A .NET Representation of a Patchouli Page for displaying Smelting Recipes
    /// <para></para>
    /// <a href="https://vazkiimods.github.io/Patchouli/docs/patchouli-basics/page-types/#smelting-recipe-pages-patchoulismelting">Patchouli Docs</a>
    /// </summary>
    public class SmeltingPage : PatchouliPage
    {
        public override string Type => "patchouli:smelting";

        [JsonPropertyName("recipe")]
        public required string Recipe { get; set; }

        [JsonPropertyName("recipe2")]
        public string? Recipe2 { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
}
