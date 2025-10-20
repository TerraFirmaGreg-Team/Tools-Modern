using Common;

namespace LanguageMerger
{
    internal class LanguageMergerProgram(CommandLineOptions options, string langDir)
    {
        private readonly List<ModLocale> r_modLocales = [];
        private readonly CommandLineOptions r_options = options;
        private readonly string r_langDir = langDir;

        public async Task<bool> Run()
        {
            ConsoleLogHelper.WriteLine("Language Merger Program has Started!", LogLevel.Message);

            ConsoleLogHelper.WriteLine("Generating Mod Locales", LogLevel.Message);
            await GenerateModLocales();
            ConsoleLogHelper.WriteLine("Creating Output Directories", LogLevel.Message);
            await CreateOutputDirectories();
            ConsoleLogHelper.WriteLine("Deserializing JSON Files", LogLevel.Message);
            await DeserializeJSONFiles();
            ConsoleLogHelper.WriteLine("Merging Deserialized JSON Files", LogLevel.Message);
            await MergeLanguageDictionaries();
            ConsoleLogHelper.WriteLine("Writing Files", LogLevel.Message);
            await WriteFiles();
            ConsoleLogHelper.WriteLine("Overwriting Files", LogLevel.Message);
            await MoveFilesToKJSAssetsFolder();

            return true;
        }

        private Task GenerateModLocales()
        {
            var enumeratedDirectories = new DirectoryInfo(r_langDir).EnumerateDirectories();
            foreach (var modDirectory in enumeratedDirectories)
            {
                ModLocale locale = new ModLocale(modDirectory.Name);
                var localeFolders = modDirectory.EnumerateDirectories();

                int jsonFileCount = 0;
                foreach (var localeFolder in localeFolders)
                {
                    var languageFiles = localeFolder.EnumerateFiles("*.json", SearchOption.AllDirectories).ToList();
                    jsonFileCount = languageFiles.Count;
                    locale.r_localeToFiles.Add(localeFolder.Name, languageFiles.ToArray());
                }
                ConsoleLogHelper.WriteLine($"Created Mod Locale for {modDirectory}, found {jsonFileCount} json files", LogLevel.Info);
                r_modLocales.Add(locale);
            }
            return Task.CompletedTask;
        }

        private async Task DeserializeJSONFiles()
        {
            List<Task> tasks = [];
            foreach (ModLocale modLocale in r_modLocales)
            {
                ConsoleLogHelper.WriteLine($"Deserializing JSON files for {modLocale.r_modID}", LogLevel.Info);
                tasks.Add(modLocale.DeserializeDictionaries());
            }
            await Task.WhenAll(tasks);
        }

        private async Task MergeLanguageDictionaries()
        {
            List<Task> tasks = [];
            foreach (ModLocale locale in r_modLocales)
            {
                ConsoleLogHelper.WriteLine($"Merging JSON for {locale.r_modID}", LogLevel.Info);
                tasks.Add(locale.MergeDeserializedDictionaries());
            }
            await Task.WhenAll(tasks);
        }

        private async Task CreateOutputDirectories()
        {
            var workingDirectory = Directory.GetCurrentDirectory();
            var outputDirectory = Path.Combine(workingDirectory, "OUTPUT");

            var dirInfo = Directory.CreateDirectory(outputDirectory);

            List<Task> tasks = [];
            foreach (ModLocale locale in r_modLocales)
            {
                tasks.Add(locale.EnsureOutputDir(dirInfo));
            }
            await Task.WhenAll(tasks);
        }

        private async Task WriteFiles()
        {
            List<Task> tasks = [];
            foreach (ModLocale locale in r_modLocales)
            {
                ConsoleLogHelper.WriteLine($"Writing merged JSON for {locale.r_modID}'s Locales:\n{string.Join('\n', locale.r_localeToFiles.Keys)}", LogLevel.Info);
                tasks.Add(locale.WriteFiles());
            }
            await Task.WhenAll(tasks);
        }

        private async Task MoveFilesToKJSAssetsFolder()
        {
            List<Task> tasks = [];
            foreach (ModLocale locale in r_modLocales)
            {
                ConsoleLogHelper.WriteLine($"Moving merged JSON Files for {locale.r_modID} to the kjs assets folder.", LogLevel.Info);
                tasks.Add(locale.MoveFilesToKJSAssetsFolder(new DirectoryInfo(r_options.AssetsDir!)));
            }
            await Task.WhenAll(tasks);
        }
    }
}
