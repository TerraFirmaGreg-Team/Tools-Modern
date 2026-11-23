using System.Text.Json.Serialization;

namespace Common.Patchouli
{
	public class InstantBarrelPage : PatchouliPage
	{
		public override string Type => "tfg:instant_barrel_recipe";

		[JsonPropertyName("recipe")]
		public required string Recipe { get; set; }

		[JsonPropertyName("title")]
		public string? Title { get; set; }

		[JsonPropertyName("text")]
		public string? Text { get; set; }
	}
}
