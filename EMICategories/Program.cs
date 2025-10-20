using System.Text.Json;
using System.Text.Json.Nodes;
using Common;

var options = CommandLineOptions.Parse(Environment.GetCommandLineArgs());
if (options is null)
	return;

var modpackDir = options.ModpackDir;
var kjsAssetsDir = options.AssetsDir;

var outDir = Path.Combine(kjsAssetsDir!, "emi\\category\\properties");

Directory.CreateDirectory(outDir);

var cwd = Directory.GetCurrentDirectory();
var input = Path.Combine(cwd.Substring(0, cwd.IndexOf("EMICategories") + "EMICategories".Length), "categories.ini");

int i = 0;
foreach (var line in File.ReadAllLines(input))
{
	if (string.IsNullOrEmpty(line))
		continue;

	var doc = new JsonObject
	{
		[line] = new JsonObject
		{
			["order"] = i++
		}
	};

	File.WriteAllText(Path.Combine(outDir, line.Replace(':', '_')) + ".json", JsonSerializer.Serialize(doc));
}