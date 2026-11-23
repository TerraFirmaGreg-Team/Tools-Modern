using System.Text.Json.Serialization;

namespace Common.Patchouli
{
	public class KnappingPage : PatchouliPage
	{
		public override string Type => "tfg:knapping_recipe";

		[JsonPropertyName("recipe")]
		public required string Recipe { get; set; }

		[JsonPropertyName("title")]
		public string? Title { get; set; }

		[JsonPropertyName("text")]
		public string? Text { get; set; }
	}
}
