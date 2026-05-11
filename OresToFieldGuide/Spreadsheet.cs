using System.Drawing;
using Common;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace OresToFieldGuide
{
	public class Spreadsheet(Dictionary<Dimension, List<Vein>> veinDict, Dictionary<string, Rock> rockDict, Dictionary<string, Ore> oreDict, Translations translations)
	{
		private readonly Dictionary<Dimension, List<Vein>> m_veinDict = veinDict;
		private readonly Dictionary<string, Rock> m_rockDict = rockDict;
		private readonly Dictionary<string, Ore> m_oreDict = oreDict;
		private readonly Translations m_translations = translations;


		public void Export(string dataFolder)
		{
			var doc = new ExcelPackage();

			var greenStyle = doc.Workbook.Styles.CreateNamedStyle("greenStyle");
			greenStyle.Style.Fill.PatternType = ExcelFillStyle.Solid;
			greenStyle.Style.Fill.BackgroundColor.SetColor(Color.FromKnownColor(KnownColor.LightGreen));

			var redStyle = doc.Workbook.Styles.CreateNamedStyle("redStyle");
			redStyle.Style.Fill.PatternType = ExcelFillStyle.Solid;
			redStyle.Style.Fill.BackgroundColor.SetColor(Color.FromKnownColor(KnownColor.LightPink));

			foreach ((var dimension, var veins) in m_veinDict)
			{
				var rocks = veins.SelectMany(v => v.Rocks).Distinct().Order().ToList();

				var sheet = doc.Workbook.Worksheets.Add(dimension.ID);
				int rockColumnIndex = SetUpSheet(sheet, rocks);

				int row = 2;
				foreach (var vein in veins)
				{
					int column = 1;

					//sheet.Cells[row, column++].Value = string.Join('/', vein.Ores.Select(o => m_translations.Get("en_us", m_oreDict[o.OreID].TranslationKey)));

					sheet.Cells[row, column++].Value = vein.ID;
					sheet.Cells[row, column++].Value = vein.Type[4..];

					string distribution = "";
					foreach (var ore in vein.Ores)
					{
						distribution += $"{ore.OreID} {ore.Weight}";
						if (ore.FullBlockWeight != null)
						{
							distribution += $" [{ore.FullBlockWeight}]";
						}
						distribution += " / ";
					}
					sheet.Cells[row, column++].Value = distribution[..^3];

					sheet.Cells[row, column++].Value = vein.Config.MinY;
					sheet.Cells[row, column++].Value = vein.Config.MaxY;

					if (vein.Project)
					{
						sheet.Cells[row, column++].Value = "✔️";
					}
					else
					{
						column++;
					}

					sheet.Cells[row, column++].Value = vein.Config.Rarity;
					sheet.Cells[row, column++].Value = vein.Config.Density.ToString("P0");

					if (vein.Type is "tfc:cluster_vein" or "tfc:disc_vein")
					{
						sheet.Cells[row, column++].Value = vein.Config.Size;
					}
					else
					{
						column++;
					}

					if (vein.Type is "tfc:disc_vein" or "tfc:pipe_vein")
					{
						sheet.Cells[row, column++].Value = vein.Config.Height;
					}
					else
					{
						column++;
					}

					if (vein.Type is "tfc:pipe_vein")
					{
						sheet.Cells[row, column++].Value = vein.Config.Radius;
					}
					else
					{
						column++;
					}

					if (vein.NearLava)
					{
						sheet.Cells[row, column++].Value = "✔️";
					}
					else
					{
						column++;
					}

					sheet.Cells[row, column++].Value = vein.BiomeTag ?? "";

					if (vein.Climate is not null)
					{
						string climate = "";
						if (vein.Climate.MinRainfall is not null || vein.Climate.MaxRainfall is not null)
						{
							climate += $"Min {vein.Climate.MinRainfall ?? 0}mm, Max {vein.Climate.MaxRainfall ?? 500}mm, ";
						}
						if (vein.Climate.MinTemperature is not null)
						{
							climate += $"Min {vein.Climate.MinTemperature}C, ";
						}
						if (vein.Climate.MaxTemperature is not null)
						{
							climate += $"Max {vein.Climate.MaxTemperature}C, ";
						}
						if (vein.Climate.MinForest is not null || vein.Climate.MaxForest is not null)
						{
							climate += $"Forest {vein.Climate.MinForest ?? "none"} - {vein.Climate.MaxForest ?? "old_growth"}";
						}

						sheet.Cells[row, column++].Value = climate.TrimEnd(',', ' ');
					}
					else
					{
						column++;
					}

					foreach (var rock in rocks)
					{
						bool contains = vein.Rocks.Contains(rock);

						if (contains)
						{
							sheet.Cells[row, column].StyleName = "greenStyle";
							sheet.Cells[row, column].Value = "✔️";
						}
						else
						{
							sheet.Cells[row, column].StyleName = "redStyle";
							sheet.Cells[row, column].Value = " ";
						}

						column++;
					}

					row++;
				}

				for (int i = 1; i < rockColumnIndex + m_rockDict.Count; i++)
				{
					sheet.Column(i).AutoFit();
				}
			}

			using var outStream = new FileStream(Path.Combine(dataFolder, "sheet.xlsx"), FileMode.Create);
			doc.SaveAs(outStream);
			doc.Dispose();

			ConsoleLogHelper.WriteLine("Exported spreadsheet!", LogLevel.Info);
		}

		private int SetUpSheet(ExcelWorksheet sheet, IEnumerable<string> rocks)
		{
			int rockColumnIndex;

			int column = 1;
			sheet.Cells[1, column++].Value = "Materials";
			sheet.Cells[1, column++].Value = "Vein ID";
			sheet.Cells[1, column++].Value = "Type";
			sheet.Cells[1, column++].Value = "Distribution";
			sheet.Cells[1, column++].Value = "Min Y";
			sheet.Cells[1, column++].Value = "Max Y";
			sheet.Cells[1, column++].Value = "Projected?";
			sheet.Cells[1, column++].Value = "Rarity";
			sheet.Cells[1, column++].Value = "Density";
			sheet.Cells[1, column++].Value = "Size";
			sheet.Cells[1, column++].Value = "Height";
			sheet.Cells[1, column++].Value = "Radius";
			sheet.Cells[1, column++].Value = "Near Lava?";
			sheet.Cells[1, column++].Value = "Biome";
			sheet.Cells[1, column++].Value = "Climate";

			rockColumnIndex = column;
			foreach (var rock in rocks)
			{
				sheet.Cells[1, column++].Value = rock;
			}

			// Format as text
			sheet.Cells.Style.Numberformat.Format = "@";

			// Set alignment
			sheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
			sheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Top;

			// Make the first row bold
			var headerRowStyle = sheet.Row(1).Style;
			headerRowStyle.Font.Bold = true;
			headerRowStyle.HorizontalAlignment = ExcelHorizontalAlignment.Center;
			headerRowStyle.Fill.PatternType = ExcelFillStyle.Solid;
			headerRowStyle.Fill.BackgroundColor.SetColor(Color.DarkSlateGray);
			headerRowStyle.Font.Color.SetColor(Color.White);

			// And freeze it
			sheet.View.FreezePanes(2, 1);

			// Make the first column bold
			sheet.Column(1).Style.Font.Bold = true;

			return rockColumnIndex;
		}
	}
}
