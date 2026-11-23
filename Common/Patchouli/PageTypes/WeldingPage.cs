using System.Text.Json.Serialization;

namespace Common.Patchouli
{
	public class WeldingPage : PatchouliPage
	{
		public override string Type => "tfg:welding_recipe";

		[JsonPropertyName("recipe")]
		public required string Recipe { get; set; }

		[JsonPropertyName("title")]
		public string? Title { get; set; }

		[JsonPropertyName("text")]
		public string? Text { get; set; }
	}
}
