using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
	public class Rock : IDataJsonObject
	{
		[JsonPropertyName("id")]
		public required string ID { get; set; }

		[JsonPropertyName("pattern")]
		public required string Pattern { get; set; }

		[JsonPropertyName("replaceable_blocks")]
		public required string[] ReplaceableBlocks { get; set; }

		/// <summary>
		/// igneous, sedimentary, metamorphic
		/// </summary>
		[JsonPropertyName("rock_type")]
		public required string RockType { get; set; }

		/// <summary>
		/// extrusive, intrusive, clastic, organic, carbonate
		/// </summary>
		[JsonPropertyName("rock_subtype")]
		public string? RockSubtype { get; set; }

		/// <summary>
		/// felsic, intermediate, mafic, ultramafic
		/// </summary>
		[JsonPropertyName("rock_grade")]
		public string? RockGrade { get; set; }
	}
}
