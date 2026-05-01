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
			"ru_ru", // Russian
			"uk_ua", // Ukranian
			"pt_br", // Brazilian Portuguese
			"zh_cn", // Simplified Chinese
			"fr_fr", // French
			"ja_jp", // Japanese
			"de_de", // German
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

			// 4) Write out patchouli

			ExportPatchouliEntries();

			// 5) Write out EMI vein pages & translation keys
			ExportOreTranslationKeys();
			ExportEmiOreData();

			// 6) Generate spreadsheet

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
			const int characterLimit = 27;

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

							if ((generated + next).Length > characterLimit)
							{
								generated += "...";
								break;
							}
							else
							{
								if (generated.Length != 0)
								{
									generated += ", ";
								}
								generated += next;
							}
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

		private string GetFieldGuideOutputDirectory(string locale)
		{
			return Path.Combine(m_arguments.ModpackFolder, "kubejs/assets/tfc/patchouli_books/field_guide", locale, "entries/tfg_ores");
		}

		private PatchouliEntry GenerateOreIndexForDimension(string locale, Dimension dim)
		{
			var entry = new PatchouliEntry()
			{
				FileNameWithoutExtension = $"{dim.ID}_ore_index",
				Icon = dim.OreIndexIcon,
				Name = m_translations.Get(locale, dim.OreIndexTranslationKey),
				ReadByDefault = true,
				SortNum = dim.SortOrder * 2,
				Pages = []
			};

			// First page

			int numPages = 0;

			var pageBuilder = new PatchouliStringBuilder(new StringBuilder());
			pageBuilder.Append(string.Format(m_translations.Get(locale, "tfg.ore_patchouli_info.ore_index_format"), m_translations.Get(locale, dim.NameTranslationKey)));

			entry.Pages.Add(new TextPage()
			{
				Title = m_translations.Get(locale, dim.OreIndexTranslationKey),
				Text = pageBuilder.Dump(),
			});
			numPages++;

			// Build the list of veins, sorted alphabetically, with each vein sorted by densest

			var sortedVeins = m_oreDict.Values.Select(ore =>
				(ore,
				m_veinDict[dim]
					.Where(vein =>
						vein.Ores
							.Any(wb => wb.OreID == ore.ID))
							.OrderByDescending(vein =>
								vein.Ores.Single(wb => wb.OreID == ore.ID).Weight)))
				.Where(tuple => tuple.Item2.Any())
				.OrderBy(tuple => m_translations.Get(locale, tuple.ore.TranslationKey));

			// Build the pages

			foreach (var chunk in sortedVeins.Chunk(14))
			{
				foreach ((var ore, var veins) in chunk)
				{
					pageBuilder.Append("$(li)");
					pageBuilder.Append($"{m_translations.Get(locale, ore.TranslationKey)}: ");

					foreach (var vein in veins)
					{
						var wb = vein.Ores.Single(wb => wb.OreID == ore.ID);
						pageBuilder.InternalLink($"{(int) wb.WeightPercent!}%", $"tfg_ores/{dim.ID}_vein_index", vein.ID, true);

						if (vein != veins.Last())
						{
							pageBuilder.Append(", ");
						}
					}

					pageBuilder.Append(PatchouliStringBuilder.EMPTY);
				}

				entry.Pages.Add(new TextPage()
				{
					Text = pageBuilder.Dump()
				});
				numPages++;
			}

			return entry;
		}

		private PatchouliEntry GenerateVeinIndexForDimension(string locale, Dimension dim)
		{
			var entry = new PatchouliEntry()
			{
				FileNameWithoutExtension = $"{dim.ID}_vein_index",
				Icon = dim.VeinIndexIcon,
				Name = m_translations.Get(locale, dim.VeinIndexTranslationKey),
				ReadByDefault = true,
				SortNum = (dim.SortOrder * 2) + 1,
				Pages = []
			};

			// First page

			int numPages = 0;

			var pageBuilder = new PatchouliStringBuilder(new StringBuilder());
			pageBuilder.Append(string.Format(m_translations.Get(locale, "tfg.ore_patchouli_info.vein_index_format"), m_translations.Get(locale, dim.NameTranslationKey)));

			entry.Pages.Add(new TextPage()
			{
				Title = m_translations.Get(locale, dim.VeinIndexTranslationKey),
				Text = pageBuilder.Dump()
			});
			numPages++;

			// Index pages

			foreach (var chunk in m_veinDict[dim].OrderBy(v => v.TranslatedNames[locale]).Chunk(14))
			{
				foreach (var vein in chunk)
				{
					pageBuilder.Append("$(li)");
					pageBuilder.InternalLink(vein.TranslatedNames[locale], $"tfg_ores/{dim.ID}_vein_index", vein.ID);
					pageBuilder.Append(PatchouliStringBuilder.EMPTY);
				}

				entry.Pages.Add(new TextPage()
				{
					Text = pageBuilder.Dump()
				});
				numPages++;
			}

			// If there's an odd number of pages, add a blank one

			if (numPages % 2 != 0)
			{
				entry.Pages.Add(new EmptyPage());
			}

			// Vein pages

			foreach (var vein in m_veinDict[dim].OrderBy(v => v.TranslatedNames[locale]))
			{
				numPages = 0;

				// Vein info page

				pageBuilder.ThingMacro(m_translations.Get(locale, "tfg.ore_patchouli_info.rarity"));
				pageBuilder.Append($": 1/{vein.Config.Rarity} ");
				pageBuilder.Append(m_translations.Get(locale, "tfg.ore_patchouli_info.chunks"));
				pageBuilder.LineBreak();

				pageBuilder.ThingMacro(m_translations.Get(locale, "tfg.ore_patchouli_info.density"));
				pageBuilder.Append($": {Math.Round(vein.Config.Density * 100)}%");
				pageBuilder.LineBreak();

				pageBuilder.ThingMacro(m_translations.Get(locale, "tfg.ore_patchouli_info.type"));
				pageBuilder.Append($": {m_translations.Get(locale, $"tfg.ore_patchouli_info.{vein.Type[4..]}")}");
				pageBuilder.LineBreak();

				if (vein.Project)
				{
					pageBuilder.ThingMacro(m_translations.Get(locale, "tfg.ore_patchouli_info.project"));
				}
				else
				{
					pageBuilder.ThingMacro(m_translations.Get(locale, "tfg.ore_patchouli_info.y"));
				}
				pageBuilder.Append($": {vein.Config.MinY} — {vein.Config.MaxY}");
				pageBuilder.LineBreak();

				if (vein.Type == "tfc:disc_vein")
				{
					pageBuilder.ThingMacro(m_translations.Get(locale, "tfg.ore_patchouli_info.size"));
					pageBuilder.Append($": {vein.Config.Size}");
					pageBuilder.LineBreak();

					pageBuilder.ThingMacro(m_translations.Get(locale, "tfg.ore_patchouli_info.height"));
					pageBuilder.Append($": {vein.Config.Height}");
				}
				else if (vein.Type == "tfc:cluster_vein")
				{
					pageBuilder.ThingMacro(m_translations.Get(locale, "tfg.ore_patchouli_info.size"));
					pageBuilder.Append($": {vein.Config.Size}");
				}
				else // pipe vein
				{
					pageBuilder.ThingMacro(m_translations.Get(locale, "tfg.ore_patchouli_info.height"));
					pageBuilder.Append($": {vein.Config.Height}");
					pageBuilder.LineBreak();

					pageBuilder.ThingMacro(m_translations.Get(locale, "tfg.ore_patchouli_info.radius"));
					pageBuilder.Append($": {vein.Config.Radius}");
				}

				if (vein.Indicator!.Depth > 1)
				{
					pageBuilder.LineBreak();
					pageBuilder.ThingMacro(m_translations.Get(locale, "tfg.ore_patchouli_info.indicator_depth"));
					pageBuilder.Append($": {vein.Indicator!.Depth}");
				}

				if (vein.NearLava)
				{
					pageBuilder.LineBreak();
					pageBuilder.ThingMacro(m_translations.Get(locale, "tfg.ore_patchouli_info.near_lava"));
				}

				pageBuilder.LineBreak();
				pageBuilder.ThingMacro(m_translations.Get(locale, "tfg.ore_patchouli_info.biomes"));
				if (vein.BiomeTag is null)
				{
					pageBuilder.Append($": {m_translations.Get(locale, "tfg.ore_patchouli_info.biomes_any")}");
				}
				else
				{
					string translatedBiomeList = string.Join(", ", m_tags
						.GetBiomeTranslationKeys(vein.BiomeTag)
						.Select(biome => m_translations.Get(locale, biome)));

					pageBuilder.Append($": $(t:{translatedBiomeList}){m_translations.Get(locale, m_tags.GetTagTranslationKey(vein.BiomeTag))}$(/t)");
				}

				if (vein.Climate is not null)
				{
					if (vein.Climate.MinRainfall is not null || vein.Climate.MaxRainfall is not null)
					{
						pageBuilder.LineBreak();
						pageBuilder.ThingMacro(m_translations.Get(locale, "tfg.ore_patchouli_info.rainfall"));
						pageBuilder.Append($": {vein.Climate.MinRainfall ?? 0} - {vein.Climate.MaxRainfall ?? 500}mm");
					}

					if (vein.Climate.MinTemperature is not null && vein.Climate.MaxTemperature is null)
					{
						pageBuilder.LineBreak();
						pageBuilder.ThingMacro(m_translations.Get(locale, "tfg.ore_patchouli_info.temperature"));
						pageBuilder.Append($": {string.Format(m_translations.Get(locale, "tfg.ore_patchouli_info.temperature_and_above"), vein.Climate.MinTemperature)}");
					}
					else if (vein.Climate.MinTemperature is null && vein.Climate.MaxTemperature is not null)
					{
						pageBuilder.LineBreak();
						pageBuilder.ThingMacro(m_translations.Get(locale, "tfg.ore_patchouli_info.temperature"));
						pageBuilder.Append($": {string.Format(m_translations.Get(locale, "tfg.ore_patchouli_info.temperature_and_below"), vein.Climate.MaxTemperature)}");
					}
					else if (vein.Climate.MinTemperature is not null && vein.Climate.MaxTemperature is not null)
					{
						pageBuilder.LineBreak();
						pageBuilder.ThingMacro(m_translations.Get(locale, "tfg.ore_patchouli_info.temperature"));
						pageBuilder.Append($": {vein.Climate.MinTemperature} - {vein.Climate.MaxTemperature}°C");
					}
				}

				pageBuilder.LineBreak2();
				pageBuilder.ThingMacro(m_translations.Get(locale, "tfg.ore_patchouli_info.stone_types"));
				pageBuilder.Append($": {string.Join(", ", vein.Rocks.Select(r => m_translations.Get(locale, m_rockDict[r].TranslationKey)).Order())}");

				entry.Pages.Add(new TextPage
				{
					Title = vein.TranslatedNames[locale],
					Text = pageBuilder.Dump(),
					Anchor = vein.ID
				});
				numPages++;

				// One page per ore in the vein

				foreach (var block in vein.Ores.OrderByDescending(wb => wb.Weight))
				{
					var ore = m_oreDict[block.OreID];

					pageBuilder.ThingMacro(m_translations.Get(locale, "tfg.ore_patchouli_info.percentage"));
					pageBuilder.Append($": {(int) block.WeightPercent!}%");

					pageBuilder.LineBreak();
					pageBuilder.Append(m_translations.Get(locale, ore.InfoKey));
					
					if (ore.Formula != null)
					{
						pageBuilder.LineBreak();
						pageBuilder.ThingMacro(m_translations.Get(locale, "tfg.ore_patchouli_info.formula"));
						pageBuilder.Append($": {ore.Formula}");
					}

					if (ore.Hazard != null)
					{
						pageBuilder.LineBreak();
						pageBuilder.ThingMacro(m_translations.Get(locale, "tfg.ore_patchouli_info.hazard"));
						pageBuilder.Append(": ");
						pageBuilder.Color(m_translations.Get(locale, $"tfg.ore_patchouli_info.{ore.Hazard}"), MinecraftColorCode.Red);
					}

					entry.Pages.Add(new MultiblockPage
					{
						Name = m_translations.Get(locale, ore.TranslationKey),
						EnableVisualize = false,
						Text = pageBuilder.Dump(),
						Multiblock = ore.BuildMultiblockDisplay()
					});
					numPages++;
				}

				// If there's an odd number of pages, add a blank one

				if (numPages % 2 != 0)
				{
					entry.Pages.Add(new EmptyPage());
				}
			}

			return entry;
		}

		private void ExportPatchouliEntries()
		{
			foreach (string locale in s_locales)
			{
				// Clear out existing files
				var outputDir = GetFieldGuideOutputDirectory(locale);

				if (Directory.Exists(outputDir))
				{
					foreach (var existingPath in Directory.EnumerateFiles(outputDir, "*.json"))
					{
						if (!m_arguments.WhitelistedPatchouliEntryFilenames.Contains(Path.GetFileNameWithoutExtension(existingPath)))
						{
							File.Delete(existingPath);
						}
					}
				}
				else
				{
					Directory.CreateDirectory(outputDir);
				}

				// Then write new ones
				foreach (var dimension in m_dimensionDict.Values)
				{
					var oreIndex = GenerateOreIndexForDimension(locale, dimension);

					string oreJson = JsonSerializer.Serialize(oreIndex, m_jsonOptions);
					File.WriteAllText(Path.Combine(outputDir, oreIndex.FileNameWithoutExtension + ".json"), oreJson);
					ConsoleLogHelper.WriteLine($"Wrote out {locale} {oreIndex.FileNameWithoutExtension}", LogLevel.Info);

					var veinIndex = GenerateVeinIndexForDimension(locale, dimension);

					string veinJson = JsonSerializer.Serialize(veinIndex, m_jsonOptions);
					File.WriteAllText(Path.Combine(outputDir, veinIndex.FileNameWithoutExtension + ".json"), veinJson);
					ConsoleLogHelper.WriteLine($"Wrote out {locale} {veinIndex.FileNameWithoutExtension}", LogLevel.Info);
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
			if (!Directory.Exists(m_arguments.CoreFolder))
			{
				ConsoleLogHelper.WriteLine("Failed to find TFG-Core folder, skipping EMI Ore Data output!!", LogLevel.Warning);
				ConsoleLogHelper.WriteLine("This can result in incorrect information in EMI, please sort your shit out", LogLevel.Warning);
				return;
			}

			StringBuilder sb = new StringBuilder();
			sb.AppendLine("package su.terrafirmagreg.core.compat.emi;");
			sb.AppendLine();
			sb.AppendLine("\t// This file is automatically generated by the OresToFieldGuide program in Tools-Modern.");
			sb.AppendLine("\tpublic class ExportedOreVeinInfo {");
			sb.AppendLine("\t\tpublic static final OreVeinInfoRecipe[] RECIPES = {");
			foreach ((var dimension, var veins) in m_veinDict)
			{
				foreach (var vein in veins)
				{
					sb.AppendLine($"\t\t\tnew OreVeinInfoRecipe(\"{vein.ID}\", \"{m_dimensionDict[vein.Dimension].DimensionID}\", ")
						.AppendLine($"\t\t\t\t{vein.Config.Rarity}, {vein.Config.Density}, {vein.Config.MinY}, {vein.Config.MaxY}, {vein.Config.Size}, {vein.Config.Height}, {vein.Config.Radius}, ")
						.AppendLine($"\t\t\t\t{vein.NearLava.ToString().ToLowerInvariant()}, {vein.Project.ToString().ToLowerInvariant()}, {vein.ProjectOffset.ToString().ToLowerInvariant()}, {vein.Indicator?.Depth ?? 0}, ")
					
					// Rocks
						.Append($"\t\t\t\tnew String[] {{");

					foreach (var rock in vein.Rocks)
					{
						sb.Append($"\"{m_rockDict[rock].ReplaceableBlocks[0]}\",");
					}

					sb.AppendLine("},");

					// Ores
					sb.Append("\t\t\t\tnew OreVeinInfoRecipe.WeightedBlock[] {");

					foreach (var value in vein.Ores)
					{
						sb.Append($"new OreVeinInfoRecipe.WeightedBlock(\"{value.OreID}\", {(int) (value.WeightPercent ?? 0)}), ");
					}
					sb.AppendLine("},");

					// Biomes
					if (vein.BiomeTag is null)
					{
						sb.AppendLine("\t\t\t\tnull, null,");
					}
					else
					{
						sb.Append($"\t\t\t\t\"{vein.BiomeTag.TrimStart('#')}\", new String[] {{ ");
						sb.Append(string.Join(", ", m_tags.GetBiomeTranslationKeys(vein.BiomeTag).Select(t => $"\"{t}\"")));
						sb.AppendLine("},");
					}

					// Climate
					if (vein.Climate is null)
					{
						sb.AppendLine("\t\t\t\tnull, null, null, null,");
					}
					else
					{
						sb.Append("\t\t\t\t");
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

					//if (vein.TranslatedEmi.Any() && !string.IsNullOrWhiteSpace(vein.TranslatedEmi["en_us"]))
					//{
					//	sb.Append($"\t\t\t\tnew String[] {{");

					//	var split = vein.TranslatedEmi["en_us"]!.Split("\\n");
					//	for (int i = 0; i < split.Length; i++)
					//	{
					//		sb.Append($"\"tfg.ore_vein.{vein.ID}.emi.{i}\"");
					//		if (i + 1 < split.Length)
					//		{
					//			sb.Append(", ");
					//		}
					//	}

					//	sb.Append("}");
					//}
					//else
					//{
						sb.Append("\t\t\t\tnull");
					//}

					sb.AppendLine("),");
				}
			}
			sb.AppendLine("\t};");
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
				sb.AppendLine("\t\"tfg.ore_vein.__comment__\": \"DO NOT TRANSLATE THIS FILE. Translate the OresToFieldGuide/data/veins files instead.\",");

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