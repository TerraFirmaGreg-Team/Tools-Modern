using System.Text.Json.Serialization;

namespace Common.Patchouli
{
	public class RockKnappingPage : PatchouliPage
	{
		public override string Type => "tfg:rock_knapping_recipe"; 
		
		[JsonPropertyName("recipes")]
		public required string[] Recipes { get; set; }

		[JsonPropertyName("title")]
		public string? Title { get; set; }

		[JsonPropertyName("text")]
		public string? Text { get; set; }
	}
}
