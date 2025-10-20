using CommandLine;

namespace Common
{
	public class CommandLineOptions
	{
		[Option('m', HelpText = "Path to your \"Modpack-Modern\" directory.")]
		public string? ModpackDir { get; set; }

		[Option('a', HelpText = "Path to your \"kubejs/assets\" directory.")]
		public string? AssetsDir { get; set; }

		[Option('d', HelpText = "Path to your \"kubejs/data\" directory.")]
		public string? DataDir { get; set; }

		[Option('c', HelpText = "Path to your \"Core-Modern\" directory.")]
		public string? CoreDir { get; set; }


		public static CommandLineOptions? Parse(string[] args)
		{
			var result = Parser.Default.ParseArguments<CommandLineOptions>(args)
				.WithParsed(RunOptions)
				.WithNotParsed(ErrorOptions);

			if (!result.Errors.Any())
			{
				if (result.Value.ModpackDir is null)
				{
					Console.Error.WriteLine("ModpackDir (-m) could not be found or was not provided!");
					return default;
				}
				else if (result.Value.AssetsDir is null)
				{
					Console.Error.WriteLine("AssetsDir (-a) could not be found or was not provided!");
					return default;
				}
				else if (result.Value.DataDir is null)
				{
					Console.Error.WriteLine("DataDir (-d) could not be found or was not provided!");
					return default;
				}
			}

			return result.Value;
		}

		private static void RunOptions(CommandLineOptions options)
		{
			options.ModpackDir ??= CommonUtil.GetModpackDirectory();
			options.AssetsDir ??= CommonUtil.GetKJSAssetsFolder(options.ModpackDir);
			options.DataDir ??= CommonUtil.GetKJSDataFolder(options.ModpackDir);
			// idk how to get this programmatically
			options.CoreDir ??= "C:\\Users\\Pyritie\\IdeaProjects\\TFG-Core-Modern";
		}

		private static void ErrorOptions(IEnumerable<Error> errors)
		{
			foreach (var error in errors)
			{
				Console.Error.WriteLine(error);
			}
		}
	}
}
