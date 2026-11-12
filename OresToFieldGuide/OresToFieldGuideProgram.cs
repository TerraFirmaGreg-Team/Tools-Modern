using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Common;

namespace OresToFieldGuide
{
	internal class OresToFieldGuideProgram(ProgramArguments arguments)
	{
		public static string s_fallbackLocale => s_locales[0];
		public static readonly string[] s_locales =
		[
			"en_us", // US English
			"de_de",
			"es_es",
			"et_ee",
			"hu_hu",
			"ja_jp",
			"ko_kr",
			"lt_lt",
			"pl_pl",
			"pt_br",
			"ru_ru",
			"sv_se",
			"tr_tr",
			"uk_ua",
			"zh_cn",
			"zh_tw"
		];

		private readonly JsonSerializerOptions m_jsonOptions = new()
		{
			// Keep the file readable
			WriteIndented = true,
			// Skips anything that's null
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			// Write cyrillic characters normally
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
		};

		private readonly ProgramArguments m_arguments = arguments;
		private readonly Translations m_translations = new(s_locales, arguments);
		private readonly Tags m_tags = new(arguments);

		private readonly Dictionary<string, Dimension> m_dimensionDict = [];
		private readonly Dictionary<string, Ore> m_oreDict = [];
		private readonly Dictionary<string, Rock> m_rockDict = [];
		private readonly Dictionary<Dimension, List<Vein>> m_veinDict = [];

		public void Run()
		{
			// 1) Load json
			DeserializeData("dimensions", m_dimensionDict);
			DeserializeData("ores", m_oreDict);
			DeserializeData("rock", m_rockDict);
			DeserializeVeins();

			// 2) Reorganize data

			GenerateDefaultIndicatorsAndVeinNames();
			WeightsIntoPercents();

			// 3) Write out veins

			ExportConfiguredVeins();
			ExportPlacedVeins();
			ExportTags();

			// 4) Write out EMI vein pages & translation keys
			ExportOreTranslationKeys();
			ExportEmiOreData();

			// 5) Generate spreadsheet

			var spreadsheet = new Spreadsheet(m_veinDict, m_rockDict, m_oreDict, m_translations);
			spreadsheet.Export(m_arguments.DataFolder);
		}

		private void DeserializeData<T>(string subDir, Dictionary<string, T> dict) where T : IDataJsonObject
		{
			var paths = Path.Combine(m_arguments.DataFolder, subDir);
			foreach (var path in Directory.EnumerateFiles(paths, "*.json"))
			{
				var thing = JsonSerializer.Deserialize<T>(File.ReadAllText(path));
				if (thing == null)
				{
					ConsoleLogHelper.WriteLine($"Failed to parse ${path}, skipping", LogLevel.Error);
					continue;
				}

				dict[thing.ID] = thing;
			}
		}

		/// <summary>
		/// Deserializes all the veins and stores them in a dictionary of dims to veins
		/// </summary>
		private void DeserializeVeins()
		{
			var veinsPath = Path.Combine(m_arguments.DataFolder, "veins");
			foreach (var dimension in m_dimensionDict.Values)
			{
				List<Vein> deserializedVeins = [];

				foreach (string veinPath in Directory.EnumerateFiles(Path.Combine(veinsPath, dimension.ID), "*.json"))
				{
					Vein? vein = JsonSerializer.Deserialize<Vein>(File.ReadAllText(veinPath));
					if (vein == null)
					{
						ConsoleLogHelper.WriteLine($"Failed to parse ${veinPath}, skipping", LogLevel.Error);
						continue;
					}

					deserializedVeins.Add(vein);
				}

				m_veinDict[dimension] = deserializedVeins;
			}
		}

		private void GenerateDefaultIndicatorsAndVeinNames()
		{
			foreach (var vein in m_veinDict.Values.SelectMany(v => v))
			{
				vein.Indicator ??= IndicatorConfig.GenerateDefault(vein.Ores, m_oreDict);
				vein.Indicator.Blocks ??= IndicatorConfig.GenerateDefaultIndicatorBlocks(vein.Ores, m_oreDict);

				foreach (var locale in s_locales)
				{
					if (vein.TranslationKey is null)
					{
						// Generate a new name based off the ores in the vein
						string generated = "";
						foreach (var ore in vein.Ores)
						{
							var next = m_translations.Get(locale, m_oreDict[ore.OreID].TranslationKey);

							if (generated.Length != 0)
							{
								generated += ", ";
							}
							generated += next;
						}

						if (vein.NameSuffix is not null)
						{
							generated += " " + m_translations.Get(locale, vein.NameSuffix);
						}

						vein.TranslatedNames[locale] = generated;
					}
					else
					{
						vein.TranslatedNames[locale] = m_translations.Get(locale, vein.TranslationKey);
					}
				}
			}
		}

		private void WeightsIntoPercents()
		{
			foreach (var vein in m_veinDict.Values.SelectMany(v => v))
			{
				double totalWeight = vein.Ores.Sum(wb => wb.Weight);

				foreach (var wb in vein.Ores)
				{
					wb.WeightPercent = (wb.Weight / totalWeight) * 100.0;
				}
			}
		}

		private void ExportConfiguredVeins()
		{
			string configuredFeatureDir = Path.Combine(m_arguments.ModpackFolder, "kubejs/data/tfg/worldgen/configured_feature");

			foreach ((var dimension, var veins) in m_veinDict)
			{
				string configuredVeinDir = Path.Combine(configuredFeatureDir, dimension.ID, "vein");

				if (Directory.Exists(configuredVeinDir))
				{
					foreach (var existingPath in Directory.EnumerateFiles(configuredVeinDir, "*.json"))
					{
						if (veins.Any(v => v.ID == Path.GetFileNameWithoutExtension(existingPath)))
							continue;

						File.Delete(existingPath);
					}
				}
				else
				{
					Directory.CreateDirectory(configuredVeinDir);
				}

				foreach (var vein in veins)
				{
					var feature = new VeinConfiguredFeature(vein, m_rockDict, m_oreDict);
					string json = JsonSerializer.Serialize(feature, m_jsonOptions);
					File.WriteAllText(Path.Combine(configuredVeinDir, vein.ID + ".json"), json);
					ConsoleLogHelper.WriteLine($"Wrote out configured feature {vein.ID}", LogLevel.Info);
				}
			}
		}

		private void ExportPlacedVeins()
		{
			string placedFeatureDir = Path.Combine(m_arguments.ModpackFolder, "kubejs/data/tfg/worldgen/placed_feature");

			foreach ((var dimension, var veins) in m_veinDict)
			{
				string placedVeinDir = Path.Combine(placedFeatureDir, dimension.ID, "vein");

				if (Directory.Exists(placedVeinDir))
				{
					foreach (var existingPath in Directory.EnumerateFiles(placedVeinDir, "*.json"))
					{
						if (veins.Any(v => v.ID == Path.GetFileNameWithoutExtension(existingPath) && v.VisualOnly == true))
							continue;

						File.Delete(existingPath);
					}
				}
				else
				{
					Directory.CreateDirectory(placedVeinDir);
				}

				foreach (var vein in veins)
				{
					if (vein.VisualOnly == true)
						continue;

					// These are pretty bare-bones because tfc handles all the placement for you already
					var feature = new VeinPlacedFeature()
					{
						Feature = $"tfg:{dimension.ID}/vein/{vein.ID}",
					};

					if (vein.Climate is not null)
					{
						feature.Placement.Add(new ClimatePlacer(vein.Climate));
					}

					string json = JsonSerializer.Serialize(feature, m_jsonOptions);
					File.WriteAllText(Path.Combine(placedVeinDir, vein.ID + ".json"), json);
					ConsoleLogHelper.WriteLine($"Wrote out placed feature {vein.ID}", LogLevel.Info);
				}
			}
		}

		private void ExportEmiOreData()
		{
			if (m_arguments.CoreFolder is null || !Directory.Exists(m_arguments.CoreFolder))
			{
				ConsoleLogHelper.WriteLine("Failed to find TFG-Core folder, skipping EMI Ore Data output!!", LogLevel.Warning);
				ConsoleLogHelper.WriteLine("This can result in incorrect information in EMI, please sort your shit out", LogLevel.Warning);
				return;
			}

			StringBuilder sb = new StringBuilder();
			sb.AppendLine("package su.terrafirmagreg.core.compat.emi;");
			sb.AppendLine();
			sb.AppendLine("import com.google.common.collect.ImmutableMap;");
			sb.AppendLine("import net.minecraft.resources.ResourceLocation;");
			sb.AppendLine();
			sb.AppendLine("import java.util.Map;");
			sb.AppendLine();
			sb.AppendLine("// This file is automatically generated by the OresToFieldGuide program in Tools-Modern.");
			sb.AppendLine("public class ExportedOreVeinInfo {");
			sb.AppendLine("\tpublic static final OreVeinInfoRecipe[] RECIPES = {");
			foreach ((var dimension, var veins) in m_veinDict)
			{
				foreach (var vein in veins)
				{
					sb.AppendLine($"\t\tnew OreVeinInfoRecipe(\"{vein.ID}\", \"{m_dimensionDict[vein.Dimension].DimensionID}\", ")
						.AppendLine($"\t\t\t{vein.Config.Rarity}, {vein.Config.Density}, {vein.Config.MinY}, {vein.Config.MaxY}, {vein.Config.Size}, {vein.Config.Height}, {vein.Config.Radius}, ")
						.AppendLine($"\t\t\t{vein.NearLava.ToString().ToLowerInvariant()}, {vein.Project.ToString().ToLowerInvariant()}, {vein.ProjectOffset.ToString().ToLowerInvariant()}, {vein.Indicator?.Depth ?? 0}, ")

						// Rocks
						.Append($"\t\t\tnew String[] {{");

					foreach (var rock in vein.Rocks)
					{
						sb.Append($"\"{m_rockDict[rock].ReplaceableBlocks[0]}\",");
					}

					sb.AppendLine("},");

					// Ores
					sb.Append("\t\t\tnew OreVeinInfoRecipe.WeightedBlock[] {");

					foreach (var value in vein.Ores)
					{
						sb.Append($"new OreVeinInfoRecipe.WeightedBlock(\"{value.OreID}\", {(int) (value.WeightPercent ?? 0)}), ");
					}
					sb.AppendLine("},");

					// Biomes
					if (vein.BiomeTag is null)
					{
						sb.AppendLine("\t\t\tnull, null,");
					}
					else
					{
						sb.Append($"\t\t\t\"{vein.BiomeTag.TrimStart('#')}\", new String[] {{ ");
						sb.Append(string.Join(", ", m_tags.GetBiomeTranslationKeys(vein.BiomeTag).Select(t => $"\"{t}\"")));
						sb.AppendLine("},");
					}

					// Climate
					if (vein.Climate is null)
					{
						sb.AppendLine("\t\t\tnull, null, null, null,");
					}
					else
					{
						sb.Append("\t\t\t");
						if (vein.Climate.MinRainfall is not null || vein.Climate.MaxRainfall is not null)
						{
							sb.Append($"{vein.Climate.MinRainfall ?? 0}, {vein.Climate.MaxRainfall ?? 500}, ");
						}

						if (vein.Climate.MinTemperature is not null && vein.Climate.MaxTemperature is null)
						{
							sb.AppendLine($"{vein.Climate.MinTemperature}, null, ");
						}
						else if (vein.Climate.MinTemperature is null && vein.Climate.MaxTemperature is not null)
						{
							sb.AppendLine($"null, {vein.Climate.MaxTemperature}, ");
						}
						else if (vein.Climate.MinTemperature is not null && vein.Climate.MaxTemperature is not null)
						{
							sb.AppendLine($"{vein.Climate.MinTemperature}, {vein.Climate.MaxTemperature}, ");
						}
					}

					if (vein.Project)
					{
						sb.Append("\t\t\tnew String[] ");
						sb.Append("{ \"tfg.emi.ore_veins.projected.emi.0\", \"tfg.emi.ore_veins.projected.emi.1\" }");
					}
					else
					{
						sb.Append("\t\t\tnull");
					}

					sb.AppendLine("),");
				}
			}

			sb.AppendLine("\t};");
			sb.AppendLine();
			sb.AppendLine("\tpublic record OreVeinRockInfo(String id, ResourceLocation rawRock, String rockType, String rockSubtype, String rockGrade) { }");
			sb.AppendLine();
			sb.AppendLine("\tpublic static final Map<String, OreVeinRockInfo> ROCKS = new ImmutableMap.Builder<String, OreVeinRockInfo>()");
			foreach (var rock in m_rockDict.Values)
			{
				var block = rock.ReplaceableBlocks.First().Split(':');

				sb.AppendLine($"\t\t.put(\"{rock.ReplaceableBlocks.First()}\", new OreVeinRockInfo(\"{rock.ID}\", ResourceLocation.fromNamespaceAndPath(\"{block[0]}\", \"{block[1]}\"), \"{rock.RockType}\", \"{rock.RockSubtype}\", \"{rock.RockGrade}\"))");
			}
			sb.AppendLine("\t\t.build();");

			sb.AppendLine("}");

			File.WriteAllText(Path.Combine(m_arguments.CoreFolder, "src/main/java/su/terrafirmagreg/core/compat/emi/ExportedOreVeinInfo.java"), sb.ToString());
			ConsoleLogHelper.WriteLine("Exported EMI ore vein Java class!", LogLevel.Info);
		}

		private void ExportOreTranslationKeys()
		{
			foreach (string locale in s_locales)
			{
				var sb = new StringBuilder();
				sb.AppendLine("{");
				sb.AppendLine("\t\"tfg.ore_vein.__comment__\": \"DO NOT TRANSLATE THIS FILE!!!! It is automatically generated!!\",");

				var lastVein = m_veinDict.Values.Last().Last();
				foreach (var veins in m_veinDict.Values)
				{
					foreach (var vein in veins)
					{
						if (vein.TranslatedNames.TryGetValue(locale, out string? value))
						{
							sb.Append($"\t\"tfg.ore_vein.{vein.ID}\": \"{value}\"");

							//if (vein.TranslatedEmi.TryGetValue(locale, out string? info) && !string.IsNullOrWhiteSpace(info))
							//{
							//	int i = 0;
							//	foreach (var split in info.Split("\\n"))
							//	{
							//		sb.AppendLine(",");
							//		sb.Append($"\t\"tfg.ore_vein.{vein.ID}.emi.{i++}\": \"{split}\"");
							//	}
							//}

							if (vein == lastVein)
							{
								sb.AppendLine();
							}
							else
							{
								sb.AppendLine(",");
							}
						}
					}
				}
				sb.AppendLine("}");

				var directory = Path.Combine(m_arguments.ToolsFolder, "LanguageMerger", "LanguageFiles", "tfg", locale);
				if (Directory.Exists(directory))
				{
					File.WriteAllText(Path.Combine(directory, "ore_veins.json"), sb.ToString());
					ConsoleLogHelper.WriteLine($"Exported EMI ore vein lang keys for {locale}!", LogLevel.Info);
				}
			}
		}


		private void ExportTags()
		{
			StringBuilder builder = new();
			builder.AppendLine("// priority: 0");
			builder.AppendLine();
			builder.AppendLine("// This file was generated by OresToFieldGuide, do not manually edit");
			builder.AppendLine();
			builder.AppendLine("const registerTFGOreVeinFeatures = (event) => {");

			foreach ((var dimension, var veins) in m_veinDict)
			{
				builder.AppendLine();
				builder.AppendLine($"\t// #region {dimension.ID} ores");
				builder.AppendLine();

				foreach (var vein in veins)
				{
					if (vein.VisualOnly == true)
						continue;

					builder.AppendLine($"\tevent.add('{dimension.VeinTag}', 'tfg:{dimension.ID}/vein/{vein.ID}')");
				}

				builder.AppendLine();
				builder.AppendLine("\t// #endregion");
				builder.AppendLine();
			}

			builder.AppendLine("}");

			string tagJsPath = Path.Combine(m_arguments.ModpackFolder, "kubejs/server_scripts/tfg/tags.veins.js");

			File.WriteAllText(tagJsPath, builder.ToString());
			ConsoleLogHelper.WriteLine("Exported tags!", LogLevel.Info);
		}

	}
}