using System.Text.Json.Serialization;

namespace Common.Patchouli
{
	public class MultimultiblockPage : PatchouliPage
	{
		public override string Type => "tfg:multimultiblock";

		/// <summary>
		/// Can either be <see cref="Multiblock"/> or <see cref="string"/>
		/// </summary>
		[JsonPropertyName("multiblocks")]
		public required object[] Multiblocks { get; set; }

		[JsonPropertyName("text")]
		public string? Text { get; set; }
	}
}
