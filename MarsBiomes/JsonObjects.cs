using System;
using System.Text.Json.Serialization;

namespace MarsBiomes
{
	// dimension file

	class DimensionFile
	{
		[JsonPropertyName("type")]
		public required string Type { get; set; }

		[JsonPropertyName("generator")]
		public required KJSTFCGeneratorObject Generator { get; set; }
	}

	class KJSTFCGeneratorObject
	{
		[JsonPropertyName("type")]
		public required string Type { get; set; }

		[JsonPropertyName("event_key")]
		public required string EventKey { get; set; }

		[JsonPropertyName("settings")]
		public required TFCSettings Settings { get; set; }

		[JsonPropertyName("generator")]
		public required MCGeneratorObject Generator { get; set; }
	}

	class TFCSettings
	{
		[JsonPropertyName("flat_bedrock")]
		public required bool FlatBedrock { get; set; }

		[JsonPropertyName("spawn_distance")]
		public required int SpawnDistance { get; set; }

		[JsonPropertyName("spawn_center_x")]
		public required int SpawnCenterX { get; set; }

		[JsonPropertyName("spawn_center_z")]
		public required int SpawnCenterZ { get; set; }

		[JsonPropertyName("temperature_scale")]
		public required int TemperatureScale { get; set; }

		[JsonPropertyName("rainfall_scale")]
		public required int RainfallScale { get; set; }

		[JsonPropertyName("continentalness")]
		public required double Continentalness { get; set; }

		[JsonPropertyName("rock_layer_settings")]
		public required RockLayerSettings RockLayerSettings { get; set; }
	}

	class RockLayerSettings
	{
		[JsonPropertyName("rocks")]
		public required Dictionary<string, Dictionary<string, string>> Rocks { get; set; }

		[JsonPropertyName("layers")]
		public required RockLayer[] Layers { get; set; }

		[JsonPropertyName("bottom")]
		public required string[] Bottom { get; set; }

		[JsonPropertyName("ocean_floor")]
		public required string[] OceanFloor { get; set; }

		[JsonPropertyName("volcanic")]
		public required string[] Volcanic { get; set; }

		[JsonPropertyName("land")]
		public required string[] Land { get; set; }

		[JsonPropertyName("uplift")]
		public required string[] Uplift { get; set; }
	}

	class RockLayer
	{
		[JsonPropertyName("id")]
		public required string ID { get; set; }

		[JsonPropertyName("layers")]
		public required Dictionary<string, string> Layers { get; set; }
	}

	class MCGeneratorObject
	{
		[JsonPropertyName("type")]
		public required string Type { get; set; }

		[JsonPropertyName("biome_source")]
		public required BiomeSourceObject BiomeSource { get; set; }

		[JsonPropertyName("settings")]
		public required string Settings { get; set; }
	}

	class BiomeSourceObject
	{
		[JsonPropertyName("type")]
		public required string Type { get; set; }

		[JsonPropertyName("biomes")]
		public required List<BiomeObject> Biomes { get; set; }
	}

	class BiomeObject
	{
		[JsonPropertyName("biome")]
		public required string Name { get; set; }

		[JsonPropertyName("parameters")]
		public required ParametersObject Parameters { get; set; }
	}

	class ParametersObject
	{
		[JsonPropertyName("continentalness")]
		public required double[] Continentalness { get; set; }

		[JsonPropertyName("depth")]
		public required int Depth { get; set; }

		[JsonPropertyName("erosion")]
		public required double[] Erosion { get; set; }

		[JsonPropertyName("humidity")]
		public required double[] Humidity { get; set; }

		[JsonPropertyName("offset")]
		public required int Offset { get; set; }

		[JsonPropertyName("temperature")]
		public required double[] Temperature { get; set; }

		[JsonPropertyName("weirdness")]
		public required double[] Weirdness { get; set; }
	}

	// biome group file

	class BiomeGroupFile
	{
		[JsonPropertyName("groups")]
		public required List<BiomeGroupObject> Groups { get; set; }
	}


	class BiomeGroupObject
	{
		[JsonPropertyName("name")]
		public required string Name { get; set; }

		[JsonPropertyName("mars_biome")]
		public MarsBiomeObject? MarsBiome { get; set; }

		[JsonPropertyName("vanilla_biomes")]
		public required List<string> VanillaBiomes { get; set; }
	}

	class MarsBiomeObject
	{
		[JsonPropertyName("name")]
		public required string Name { get; set; }

		// TODO: Stone layers, gravel, sand
	}
}