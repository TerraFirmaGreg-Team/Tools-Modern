using System.Diagnostics.CodeAnalysis;
using Common;

namespace LanguageMerger;

public class MainClass
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Generating Localization Files!");

        if (!TryGetProgramArguments(args, out CommandLineOptions? opts, out string? langDir))
        {
            ConsoleLogHelper.WriteLine("Failed to get Program's Arguments, Press any key to exit...", LogLevel.Error);
            Console.ReadKey();
            return;
        }

        var programInstance = new LanguageMergerProgram(opts!, langDir!);
        bool result = false;
        try
        {
            var task = programInstance.Run();
            task.Wait();

            result = task.Result;
        }
        catch (Exception e)
        {
            ConsoleLogHelper.WriteLine($"Exception has been thrown. {e}", LogLevel.Fatal);
            result = false;
        }

        if (result)
        {
            ConsoleLogHelper.WriteLine("Success :D! Press any key to exit...", LogLevel.Info);
        }
        else
        {
            ConsoleLogHelper.WriteLine("Failure D:! Press any key to exit...", LogLevel.Error);
        }
        Console.ReadKey();
    }

    private static bool TryGetProgramArguments(string[] args, out CommandLineOptions? options, out string? langDir)
    {
        options = CommandLineOptions.Parse(args);
        if (options is null)
        {
            langDir = null;
            return false;
        }

        langDir = GetLanguageFilesFolder(options);
        if (langDir is null)
        {
            return false;
        }

        return true;
    }

    private static string? GetLanguageFilesFolder(CommandLineOptions options)
    {
        var cwd = Directory.GetCurrentDirectory();

        var projectFolder = cwd.Substring(0, cwd.IndexOf("LanguageMerger") + "LanguageMerger".Length);

        string languageFilesFolder = Path.Combine(projectFolder, "LanguageFiles");
        if (!Directory.Exists(languageFilesFolder))
        {
            Console.Error.WriteLine($"The \"LanguageFiles\" folder was not found in {languageFilesFolder}");
            return null;
        }

        return languageFilesFolder;
    }
}