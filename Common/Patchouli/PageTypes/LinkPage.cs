using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
    /// <summary>
    /// A .NET Representation of a Patchouli Page for displaying Links
    /// <para></para>
    /// <a href="https://vazkiimods.github.io/Patchouli/docs/patchouli-basics/page-types/#link-pages-patchoulilink">Patchouli Docs</a>
    /// </summary>
    public class LinkPage : TextPage
    {
        public override string Type => "patchouli:link";

        [JsonPropertyName("url")]
        public required string URL { get; set; }

        [JsonPropertyName("link_text")]
        public required string LinkText { get; set; }
    }
}
