using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
	public class Vein
	{
		/// <summary>
		/// A unique identifier for this vein. Also used for the "random_name" property as a seed.
		/// </summary>
		[JsonPropertyName("id")]
		public required string ID { get; set; }

		/// <summary>
		/// The ID of the dimension this vein is in.
		/// </summary>
		[JsonPropertyName("dimension")]
		public required string Dimension { get; set; }

		/// <summary>
		/// What type of Vein this is, IE: tfc:disc_vein
		/// </summary>
		[JsonPropertyName("type")]
		public required string Type { get; set; }

		/// <summary>
		/// (Optional) Set this to true to skip writing out the placed feature and tag.
		/// Useful for veins that you want to control precisely but still show up
		/// in the field guide/EMI
		/// </summary>
		[JsonPropertyName("visual_only")]
		public bool? VisualOnly { get; set; }

		/// <summary>
		/// (Optional) Biome tag to restrict placement of this vein.
		/// </summary>
		[JsonPropertyName("biome_tag")]
		public string? BiomeTag { get; set; }

		/// <summary>
		/// (Optional) The vein will only spawn if there's lava blocks nearby
		/// </summary>
		[JsonPropertyName("near_lava")]
		public bool NearLava { get; set; } = false;

		/// <summary>
		/// (Optional) This adds the height of the world surface to the position
		/// the vein will spawn at. This means that min_y and max_y must then be 
		/// specified relative to the world surface, rather than as an absolute y level.
		/// </summary>
		[JsonPropertyName("project")]
		public bool Project { get; set; } = false;

		/// <summary>
		/// (Optional) This offsets the surface height used to make the surface projection
		/// when `project` is enabled by a random amount laterally. 
		/// For example, this might mean that a vein set to spawn a few blocks below the
		/// surface may occasionally break the surface, since the projection it is using
		/// is no longer completely accurate.
		/// </summary>
		[JsonPropertyName("project_offset")]
		public bool ProjectOffset { get; set; } = false;

		/// <summary>
		/// (Optional) Climate placer
		/// </summary>
		[JsonPropertyName("climate")]
		public ClimateConfig? Climate { get; set; }

		/// <summary>
		/// The Configuration for the Vein
		/// </summary>
		[JsonPropertyName("config")]
		public required VeinConfig Config { get; set; }

		/// <summary>
		/// The ores and their weights
		/// </summary>
		[JsonPropertyName("ores")]
		public required WeightedBlock[] Ores { get; set; }

		/// <summary>
		/// An array of <see cref="Rock.ID"/>s that this vein can appear in
		/// </summary>
		[JsonPropertyName("rocks")]
		public required string[] Rocks { get; set; }

		[JsonPropertyName("indicator")]
		public IndicatorConfig? Indicator { get; set; }

		/// <summary>
		/// Localized names and information for the ore.
		/// </summary>
		[JsonPropertyName("translations")]
		public required Translation[] RawTranslations { get; set; }

		/// <summary>
		/// Dictionary of <see cref="Translation.Language"/>, <see cref="Translation.Text"/>
		/// </summary>
		[JsonIgnore]
		public Dictionary<string, string> TranslatedNames { get; } = [];

		/// <summary>
		/// Dictionary of <see cref="Translation.Language"/>, <see cref="Translation.Info"/>
		/// </summary>
		[JsonIgnore]
		public Dictionary<string, string?> TranslatedInfo { get; } = [];

		/// <summary>
		/// Dictionary of <see cref="Translation.Language"/>, <see cref="Translation.Emi"/>
		/// </summary>
		[JsonIgnore]
		public Dictionary<string, string?> TranslatedEmi { get; } = [];
	}

	public class VeinConfig
	{
		/// <summary>
		/// The Rarity of the vein
		/// </summary>
		[JsonPropertyName("rarity")]
		public int Rarity { get; set; }

		/// <summary>
		/// The Density of the vein
		/// </summary>
		[JsonPropertyName("density")]
		public double Density { get; set; }

		/// <summary>
		/// The min Y value at which this vein can spawn
		/// </summary>
		[JsonPropertyName("min_y")]
		public int MinY { get; set; }

		/// <summary>
		/// The max Y value at which this vein can spawn
		/// </summary>
		[JsonPropertyName("max_y")]
		public int MaxY { get; set; }

		/// <summary>
		/// The Size of the Vein
		/// </summary>
		[JsonPropertyName("size")]
		public int Size { get; set; }

		/// <summary>
		/// The Height of the Vein
		/// </summary>
		[JsonPropertyName("height")]
		public int Height { get; set; }

		/// <summary>
		/// The Radius of the Vein
		/// </summary>
		[JsonPropertyName("radius")]
		public int Radius { get; set; }

		// A bunch of other properties for pipe veins that don't need to be written in the field guide
		// (Read the tfc docs on how they work)
		[JsonPropertyName("min_skew")]
		public int MinSkew { get; set; }

		[JsonPropertyName("max_skew")]
		public int MaxSkew { get; set; }

		[JsonPropertyName("min_slant")]
		public int MinSlant { get; set; }

		[JsonPropertyName("max_slant")]
		public int MaxSlant { get; set; }

		[JsonPropertyName("sign")]
		public double Sign { get; set; }
	}

	/// <summary>
	/// See https://terrafirmacraft.github.io/Documentation/1.20.x/worldgen/decorators/#climate
	/// </summary>
	public class ClimateConfig
	{
		[JsonPropertyName("min_temperature")]
		public int? MinTemperature { get; set; }

		[JsonPropertyName("max_temperature")]
		public int? MaxTemperature { get; set; }

		[JsonPropertyName("min_rainfall")]
		public int? MinRainfall { get; set; }

		[JsonPropertyName("max_rainfall")]
		public int? MaxRainfall { get; set; }

		[JsonPropertyName("min_forest")]
		public string? MinForest { get; set; }

		[JsonPropertyName("max_forest")]
		public string? MaxForest { get; set; }

		[JsonPropertyName("fuzzy")]
		public bool? Fuzzy { get; set; }
	}

	public class WeightedBlock
	{
		/// <summary>
		/// The <see cref="Ore.ID"/> of the ore to use
		/// </summary>
		[JsonPropertyName("ore")]
		public required string OreID { get; set; }

		/// <summary>
		/// The weight for this block, ranges between 0 and 100
		/// </summary>
		[JsonPropertyName("weight")]
		public required int Weight { get; set; }

		/// <summary>
		/// If this is non-null, add another entry to the vein with this weight and this ore's <see cref="Ore.FullOreBlock"/>
		/// </summary>
		[JsonPropertyName("block_weight")]
		public int? FullBlockWeight { get; set; }

		/// <summary>
		/// The weight of this ore, calculated into a percentage via
		/// <see cref="OresToFieldGuideProgram.WeightsIntoPercents"/>
		/// </summary>
		[JsonIgnore]
		public double? WeightPercent { get; set; }
	}

	public class IndicatorConfig
	{
		/// <summary>
		/// The rarity of how often this indicator appears on the surface, as 1 in N blocks
		/// </summary>
		[JsonPropertyName("rarity")]
		public int Rarity { get; set; } = 15;

		/// <summary>
		/// How close the vein has to be to the surface for it to get surface indicators
		/// </summary>
		[JsonPropertyName("depth")]
		public int Depth { get; set; } = 20;

		/// <summary>
		/// The rarity of how often this indicator appears within caves, as 1 in N blocks
		/// (This includes air blocks! So this number should always be significantly higher than <see cref="Rarity"/>)
		/// </summary>
		[JsonPropertyName("underground_rarity")]
		public int UndergroundRarity { get; set; } = 40;

		/// <summary>
		/// How many times placing an underground indicator should be attempted
		/// </summary>
		[JsonPropertyName("underground_count")]
		public int UndergroundCount { get; set; } = 200;

		/// <summary>
		/// The indicator blocks and their weights to use for this vein.
		/// Omit to generate defaults using <see cref="Ore.DefaultIndicator"/> and <see cref="WeightedBlock.Weight"/>
		/// </summary>
		[JsonPropertyName("blocks")]
		public WeightedIndicator[]? Blocks { get; set; }

		public static IndicatorConfig GenerateDefault(WeightedBlock[] ores, Dictionary<string, Ore> oreDict)
		{
			return new IndicatorConfig
			{
				Rarity = 15,
				Depth = 20,
				UndergroundRarity = 40,
				UndergroundCount = 200,
				Blocks = GenerateDefaultIndicatorBlocks(ores, oreDict)
			};
		}

		public static WeightedIndicator[] GenerateDefaultIndicatorBlocks(WeightedBlock[] ores, Dictionary<string, Ore> oreDict)
		{
			return ores.Select(wb => new WeightedIndicator
			{
				Block = oreDict[wb.OreID].DefaultIndicator ?? "minecraft:air",
				Weight = wb.Weight
			}).ToArray();
		}
	}

	public class WeightedIndicator
	{
		/// <summary>
		/// A block ID for what to use as an indicator, like "gtceu:coal_indicator" or "gtceu:ruby_bud_indicator" or "tfc:ore/small_tetrahedrite"
		/// </summary>
		[JsonPropertyName("block")]
		public required string Block { get; set; }

		/// <summary>
		/// The weight for this block, does not correlate to percentage
		/// </summary>
		[JsonPropertyName("weight")]
		public required int Weight { get; set; }
	}
}