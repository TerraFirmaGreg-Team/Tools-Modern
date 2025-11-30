using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
	public class Translation
	{
		[JsonPropertyName("lang")]
		public required string Language { get; set; }

		[JsonPropertyName("text")]
		public required string Text { get; set; }

		[JsonPropertyName("info")]
		public string? Info { get; set; }

		[JsonPropertyName("emi")]
		public string? Emi { get; set; }
	}
}
