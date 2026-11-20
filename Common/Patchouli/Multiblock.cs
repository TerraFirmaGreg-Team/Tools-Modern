using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
    /// <summary>
    /// A JSON Object used to define a MultiBlock structure in Patchouli
    /// <para></para>
    /// <a href="https://vazkiimods.github.io/Patchouli/docs/patchouli-basics/multiblocks">Patchouli Docs</a>
    /// </summary>
    public class Multiblock
    {
        [JsonPropertyName("mapping")]
        public required Dictionary<string, string> Mapping { get; set; }

        [JsonPropertyName("pattern")]
        public required string[][] Pattern { get; set; }

        [JsonPropertyName("symmetrical")]
        public bool? Symmetrical { get; set; } = null;

        [JsonPropertyName("offset")]
        public int[]? Offset { get; set; }
    }
}
