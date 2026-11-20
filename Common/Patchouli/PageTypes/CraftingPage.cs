using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
    /// <summary>
    /// A .NET Representation of a Patchouli Page for Crafting Recipes
    /// <para></para>
    /// <a href="https://vazkiimods.github.io/Patchouli/docs/patchouli-basics/page-types#crafting-recipe-pages-patchoulicrafting">Patchouli Docs</a>
    /// </summary>
    public class CraftingPage : PatchouliPage
    {
        public override string Type => "patchouli:crafting";

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
