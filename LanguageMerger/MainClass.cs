using Common;

namespace LanguageMerger;

public class MainClass
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Generating Localization Files!");

        if (!TryGetProgramArguments(out ProgramArguments programArguments))
        {
            ConsoleLogHelper.WriteLine("Failed to get Program's Arguments, Press any key to exit...", LogLevel.Error);
            Console.ReadKey();
            return;
        }

        ConsoleLogHelper.WriteLine("Arguments have been obtained! Printing...", LogLevel.Info);
        ConsoleLogHelper.WriteLine(programArguments.ToString(), LogLevel.Message);

        var programInstance = new LanguageMergerProgram(programArguments);
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


    private static bool TryGetProgramArguments(out ProgramArguments programArguments)
    {
        programArguments = new ProgramArguments();
        try
        {
            programArguments.ModpackDirectory = CommonUtil.GetModpackDirectory();
            programArguments.KjsAssetsFolder = CommonUtil.GetKJSAssetsFolder(programArguments.ModpackDirectory);
            programArguments.LanguageFilesFolder = GetLanguageFilesFolder();
        }
        catch (Exception e)
        {
            ConsoleLogHelper.WriteLine($"Exception has been thrown. {e}", LogLevel.Fatal);
            return false;
        }
        return true;
    }

    private static DirectoryInfo GetLanguageFilesFolder()
    {
        var cwd = Directory.GetCurrentDirectory();

        var projectFolder = cwd.Substring(0, cwd.IndexOf("LanguageMerger") + "LanguageMerger".Length);

        string languageFilesFolder = Path.Combine(projectFolder, "LanguageFiles");
        if (!Directory.Exists(languageFilesFolder))
        {
            throw new DirectoryNotFoundException($"The \"LanguageFiles\" folder was not found in {languageFilesFolder}");
        }

        return new DirectoryInfo(languageFilesFolder);
    }
}