using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
	public class Translation
	{
		[JsonPropertyName("info")]
		public string? Info { get; set; }

		[JsonPropertyName("emi")]
		public string? Emi { get; set; }
	}
}
