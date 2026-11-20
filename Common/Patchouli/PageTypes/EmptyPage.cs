using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
    /// <summary>
    /// A .NET Representation of a Patchouli Page that's Empty
    /// <para></para>
    /// <a href="https://vazkiimods.github.io/Patchouli/docs/patchouli-basics/page-types/#empty-pages-patchouliempty">Patchouli Docs</a>
    /// </summary>
    public class EmptyPage : PatchouliPage
    {
        public override string Type => "patchouli:empty";

        [JsonPropertyName("draw_filler")]
        public bool DrawFiller { get; set; } = true;
    }
}
