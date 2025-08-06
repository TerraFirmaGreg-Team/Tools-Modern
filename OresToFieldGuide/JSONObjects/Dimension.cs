using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
	public class Dimension : IDataJsonObject
	{
		[JsonPropertyName("id")]
		public required string ID { get; set; }

		[JsonPropertyName("sort_order")]
		public required int SortOrder { get; set; }

		[JsonPropertyName("dimension_id")]
		public required string DimensionID { get; set; }

		[JsonPropertyName("ore_index_icon")]
		public required string OreIndexIcon { get; set; }

		[JsonPropertyName("vein_index_icon")]
		public required string VeinIndexIcon { get; set; }

		[JsonPropertyName("vein_tag")]
		public required string VeinTag { get; set; }

		[JsonPropertyName("name_translations")]
		public required Dictionary<string, string> NameTranslations { get; set; }

		[JsonPropertyName("ore_index_translations")]
		public required Dictionary<string, string> OreIndexTranslations { get; set; }

		[JsonPropertyName("vein_index_translations")]
		public required Dictionary<string, string> VeinIndexTranslations { get; set; }
	}
}
