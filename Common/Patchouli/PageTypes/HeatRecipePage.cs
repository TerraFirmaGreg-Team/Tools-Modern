using System.Text.Json.Serialization;

namespace Common.Patchouli
{
	public class HeatRecipePage : PatchouliPage
	{
		public override string Type => "tfg:heat_recipe";

		[JsonPropertyName("recipe")]
		public required string Recipe { get; set; }

		[JsonPropertyName("text")]
		public string? Text { get; set; }
	}
}
