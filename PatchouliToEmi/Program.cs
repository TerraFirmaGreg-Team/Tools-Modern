using Common;

namespace PatchouliToEmi
{
	public class MainClass
	{

		public static void Main(string[] args)
		{
			if (!TryGetProgramArguments(args, out ProgramArguments programArguments))
			{
				ConsoleLogHelper.WriteLine("Failed to get Program's Arguments, Press any key to exit...", LogLevel.Error);
				Console.ReadKey();
				return;
			}

			string patchouliFolder = Path.Combine(programArguments.ModpackFolder, "kubejs", "assets", "tfg", "patchouli_books", "field_guide", "en_us", "entries");
			if (!Directory.Exists(patchouliFolder))
			{
				ConsoleLogHelper.WriteLine("Failed to find field guide entry folder", LogLevel.Fatal);
				return;
			}


		}


		private static bool TryGetProgramArguments(string[] args, out ProgramArguments programArguments)
		{
			programArguments = new ProgramArguments();

			var options = CommandLineOptions.Parse(args);
			if (options is null)
				return false;

			try
			{
				programArguments.ModpackFolder = options.ModpackDir!;
			}
			catch (Exception e)
			{
				ConsoleLogHelper.WriteLine("An Exception has been Thrown! " + e, LogLevel.Fatal);
				return false;
			}
			return true;
		}
	}


	/// <summary>
	/// Represents the arguments used by the program
	/// </summary>
	public struct ProgramArguments
	{
		/// <summary>
		/// The path to the .minecraft folder
		/// </summary>
		public string ModpackFolder;

	}
}