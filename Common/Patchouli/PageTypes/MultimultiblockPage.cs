using System.Text.Json.Serialization;

namespace Common.Patchouli
{
	public class MultimultiblockPage : PatchouliPage
	{
		public override string Type => "tfg:multimultiblock";

		[JsonPropertyName("multiblocks")]
		public required Multiblock[] Multiblocks { get; set; }

		[JsonPropertyName("text")]
		public string? Text { get; set; }
	}
}
