namespace OresToFieldGuide
{
    /// <summary>
    /// A .NET Representation of a Patchouli Page for displaying Relations
    /// <para></para>
    /// <a href="https://vazkiimods.github.io/Patchouli/docs/patchouli-basics/page-types/#relations-pages-patchoulirelations">Patchouli Docs</a>
    /// </summary>
    public class RelationsPage : PatchouliPage
    {
        public override string Type => "patchouli:relations";

        public required string[] Entries { get; set; }
        public string? Title { get; set; }
        public string? Text { get; set; }
    }
}
