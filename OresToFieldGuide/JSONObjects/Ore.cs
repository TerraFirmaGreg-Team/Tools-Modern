using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
	public class Ore : IDataJsonObject
	{
		[JsonPropertyName("id")]
		public required string ID { get; set; }

		/// <summary>
		/// Optional chemical formula of this ore.
		/// </summary>
		[JsonPropertyName("formula")]
		public string? Formula { get; set; }

		/// <summary>
		/// The name of the hazard associated with this ore, if any.
		/// </summary>
		[JsonPropertyName("hazard")]
		public string? Hazard { get; set; }

		/// <summary>
		/// If this is set, only use this block in veins instead of the gregtech generated one.
		/// </summary>
		[JsonPropertyName("ore_override")]
		public string? OreOverride { get; set; }

		/// <summary>
		/// Optional full block of ore to use in rich veins.
		/// </summary>
		[JsonPropertyName("ore_block")]
		public string? FullOreBlock { get; set; }

		/// <summary>
		/// The default indicator to use for this ore.
		/// </summary>
		[JsonPropertyName("indicator")]
		public string? DefaultIndicator { get; set; }

		/// <summary>
		/// Translation key for the ore.
		/// </summary>
		[JsonPropertyName("translation")]
		public required string TranslationKey { get; set; }

		/// <summary>
		/// Translation key for what you can do with this ore.
		/// </summary>
		[JsonPropertyName("info")]
		public required string InfoKey { get; set; }

		public Multiblock BuildMultiblockDisplay()
		{
			return new Multiblock()
			{
				Mapping = new Dictionary<string, string>
				{
					["0"] = OreOverride ?? $"#forge:ores/{ID}"
				},
				Pattern = [
					["0"],
					[" "]
				]
			};
		}
	}
}
