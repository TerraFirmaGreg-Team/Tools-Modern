using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
    /// <summary>
    /// A .NET Representation of a Patchouli Page for displaying Entities
    /// <para></para>
    /// <a href="https://vazkiimods.github.io/Patchouli/docs/patchouli-basics/page-types/#entity-pages-patchoulientity">Patchouli Docs</a>
    /// </summary>
    public class EntityPage : PatchouliPage
    {
        public override string Type => "patchouli:entity";

        [JsonPropertyName("entity")]
        public required string Entity { get; set; }

        [JsonPropertyName("scale")]
        public float Scale { get; set; } = 1.0f;

        [JsonPropertyName("offset")]
        public float Offset { get; set; } = 0;

        [JsonPropertyName("rotate")]
        public bool Rotate { get; set; } = true;

        [JsonPropertyName("default_rotation")]
        public float DefaultRotation { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
}
