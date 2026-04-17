using System.Text.Json;
using Common;

namespace OresToFieldGuide
{
	public class Translations
	{
		// <locale, <lang ID, translation>>
		private readonly Dictionary<string, Dictionary<string, string>> r_translations;

		public Translations(string[] locales, ProgramArguments args)
		{
			r_translations = [];
			foreach (var locale in locales)
			{
				ConsoleLogHelper.WriteLine($"Reading all translations for {locale}...", LogLevel.Info);

				var localeDict = new Dictionary<string, string>();
				r_translations[locale] = localeDict;

				var assetsDir = Path.Combine(args.ModpackFolder, "kubejs/assets");
				var allFiles = Directory.EnumerateFiles(assetsDir, $"{locale}.json", SearchOption.AllDirectories);

				foreach (var file in allFiles)
				{
					localeDict = r_translations[locale];

					var contents = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(file));
					foreach (var line in contents!)
					{
						localeDict[line.Key] = line.Value;
					}
				}
			}
		}

		public string Get(string locale, string id)
		{
			if (r_translations[locale].TryGetValue(id, out var translation))
			{
				return translation;
			}
			else
			{
				// It's ok if this throws if the lang doesn't exist, because that means we forgot something
				return r_translations[OresToFieldGuideProgram.s_fallbackLocale][id];
			}
		}
	}
}
