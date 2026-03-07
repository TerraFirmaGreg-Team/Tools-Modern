using System.Text.Encodings.Web;
using System.Text.Json;
using Common;

namespace LanguageMerger
{
	public class ModLocale(string modID)
	{
		public readonly string r_modID = modID;
		public readonly Dictionary<string, FileInfo[]> r_localeToFiles = [];

		private DirectoryInfo? m_modLocaleOutputFolder;
		private readonly List<Dictionary<string, string>> m_deserializedDictionaries = [];
		private readonly Dictionary<string, string> m_mergedLanguageDictionary = [];

		private readonly JsonSerializerOptions r_options = new()
		{
			AllowTrailingCommas = true,
			ReadCommentHandling = JsonCommentHandling.Skip,
			WriteIndented = true,
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
		};

		public async Task DeserializeDictionaries()
		{
			foreach (var kvp in r_localeToFiles)
			{
				var locale = kvp.Key;
				var files = kvp.Value;

				foreach (var file in files)
				{
					string json = await File.ReadAllTextAsync(file.FullName);

					//return an empty dict for empty json files.
					m_deserializedDictionaries.Add(JsonSerializer.Deserialize<Dictionary<string, string>>(json, r_options) ?? []);
				}
			}
		}

		public Task MergeDeserializedDictionaries()
		{
			Dictionary<string, string> mergedDictionary = [];

			foreach (var innerDict in m_deserializedDictionaries)
			{
				foreach ((var innerKey, var innerValue) in innerDict)
				{
					if (innerKey.StartsWith("__"))
						continue;

					if (m_mergedLanguageDictionary.TryGetValue(innerKey, out string? mergedValue))
					{
						ConsoleLogHelper.WriteLine($"Key \"{innerKey}\" already exists in the merged dictionary! Value will be overwritten. Original value: {mergedValue}", LogLevel.Warning);
					}
					m_mergedLanguageDictionary[innerKey] = innerValue;
				}
			}
			return Task.CompletedTask;
		}

		public Task EnsureOutputDir(DirectoryInfo mainOutputDir)
		{
			var modIdDirectory = mainOutputDir.CreateSubdirectory(r_modID);
			m_modLocaleOutputFolder = modIdDirectory.CreateSubdirectory("lang");

			return Task.CompletedTask;
		}

		public async Task WriteFiles()
		{
			List<StreamWriter> writers = [];
			List<Task> tasks = [];

			foreach ((var locale, var finishedDictionary) in m_mergedLanguageDictionary)
			{
				var fileOutput = Path.Combine(m_modLocaleOutputFolder!.FullName, $"{locale}.json");
				//delete the previous file.
				File.Delete(fileOutput);

				StreamWriter writer = File.CreateText(fileOutput);
				writers.Add(writer);
				var json = JsonSerializer.Serialize(finishedDictionary, r_options);
				tasks.Add(writer.WriteAsync(json));
			}
			await Task.WhenAll(tasks);

			foreach (var writer in writers)
			{
				writer.Dispose();
			}
		}

		public Task MoveFilesToKJSAssetsFolder(DirectoryInfo kjsAssetsFolder)
		{
			var modIdDirectory = kjsAssetsFolder.CreateSubdirectory(r_modID);
			var langDirectory = modIdDirectory.CreateSubdirectory("lang");

			foreach (var file in m_modLocaleOutputFolder!.EnumerateFiles("*.json"))
			{
				file.CopyTo(Path.Combine(langDirectory.FullName, file.Name), true);
			}
			return Task.CompletedTask;
		}
	}
}
