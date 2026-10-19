using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
	public class Dimension : IDataJsonObject
	{
		[JsonPropertyName("id")]
		public required string ID { get; set; }

		[JsonPropertyName("dimension_id")]
		public required string DimensionID { get; set; }

		[JsonPropertyName("vein_tag")]
		public required string VeinTag { get; set; }
	}
}
