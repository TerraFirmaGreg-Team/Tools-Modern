using System.Text.Json;
using Common;

namespace OresToFieldGuide
{
	public class Tags
	{
		/// <summary>
		/// &lt;tag, list of biomes&gt;
		/// <br />
		/// tag is formatted like `tfg:earth/is_cold` 
		/// </summary>
		private readonly Dictionary<string, List<string>> r_tags;

		public Tags(ProgramArguments args)
		{
			r_tags = [];
			ConsoleLogHelper.WriteLine("Finding all biome tags...", LogLevel.Info);

			if (args.CoreFolder is not null && Directory.Exists(args.CoreFolder))
			{
				var biomeTagDir = Path.Combine(args.CoreFolder, "src/main/resources/data/tfg/tags/worldgen/biome");
				foreach (var file in Directory.EnumerateFiles(biomeTagDir, "*.json", SearchOption.AllDirectories))
				{
					var tagName = $"tfg:{Path.GetFileName(Path.GetDirectoryName(file))}/{Path.GetFileNameWithoutExtension(file)}";

					var tag = JsonSerializer.Deserialize<Tag>(File.ReadAllText(file));
					r_tags[tagName] = tag!.Values;
				}
			}
		}

		public string GetTagTranslationKey(string tagKey)
		{
			return tagKey.TrimStart('#').Replace("tfg:", "tfg.ore_vein_tag.").Replace('/', '.');
		}

		public IEnumerable<string> GetBiomes(string tagKey)
		{
			return r_tags[tagKey.TrimStart('#')];
		}

		public IEnumerable<string> GetBiomeTranslationKeys(string tagKey)
		{
			return GetBiomes(tagKey).Select(biome => biome.Replace("tfg:", "biome.tfg."));
		}
	}
}
