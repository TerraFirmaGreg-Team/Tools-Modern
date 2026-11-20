using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
    /// <summary>
    /// A .NET Representation of a Patchouli Page for displaying Multiblocks
    /// <para></para>
    /// See also <see cref="Multiblock"/>
    /// <br></br>
    /// <a href="https://vazkiimods.github.io/Patchouli/docs/patchouli-basics/page-types/#multiblock-pages-patchoulimultiblock">Patchouli Docs</a>
    /// </summary>
    public class MultiblockPage : PatchouliPage
    {
        public override string Type => "patchouli:multiblock";

        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("multiblock")]
        public required Multiblock Multiblock { get; set; }

        [JsonPropertyName("enable_visualize")]
        public bool EnableVisualize { get; set; } = true;

        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
}
