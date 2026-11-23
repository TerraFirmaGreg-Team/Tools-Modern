using System.Text.Json.Serialization;

namespace Common.Patchouli
{
	public class SealedBarrelPage : PatchouliPage
	{
		public override string Type => "tfg:sealed_barrel_recipe";

		[JsonPropertyName("recipe")]
		public required string Recipe { get; set; }

		[JsonPropertyName("title")]
		public string? Title { get; set; }

		[JsonPropertyName("text")]
		public string? Text { get; set; }
	}
}
