using System.IO;
using System.Text.Json;
using Common;

namespace MarsBiomes
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var options = CommandLineOptions.Parse(args);
			if (options is null)
				return;

			var modpackDir = options.ModpackDir!;

			// Read inputs

			var cwd = Directory.GetCurrentDirectory();
			var inputPath = cwd.Substring(0, cwd.IndexOf("MarsBiomes") + "MarsBiomes".Length);

			var dim = JsonSerializer.Deserialize<DimensionFile>(File.ReadAllText(Path.Combine(inputPath, "unmodified_dimension.json")))!;
			var groups = JsonSerializer.Deserialize<BiomeGroupFile>(File.ReadAllText(Path.Combine(inputPath, "biome_groups.json")))!;


			// Swap vanilla biomes for tfg ones

			foreach (var biome in dim.Generator.Generator.BiomeSource.Biomes.ToList())
			{
				if (biome.Parameters.Depth != 0)
				{
					dim.Generator.Generator.BiomeSource.Biomes.Remove(biome);
					continue;
				}

				foreach (var group in groups.Groups)
				{
					if (group.VanillaBiomes.Contains(biome.Name) && group.MarsBiome != null)
					{
						biome.Name = group.MarsBiome.Name;
					}
				}
			}

			// Outputs

			var serializerOptions = new JsonSerializerOptions
			{
				WriteIndented = true
			};

			File.WriteAllText(Path.Combine(options.DataDir!, "ad_astra/dimension/mars.json"), JsonSerializer.Serialize(dim, serializerOptions));
		}
	}
}