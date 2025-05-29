using System.Text;

namespace LanguageMerger
{
    internal struct ProgramArguments
    {
        public string ModpackDirectory;

        public DirectoryInfo LanguageFilesFolder;

        public string KjsAssetsFolder;

        /// <summary>
        /// Returns the arguments in a human readable format.
        /// </summary>
        public override string ToString()
        {
            StringBuilder stringBuilder = new();

            stringBuilder.AppendLine($"Modpack-Modern Folder Path: \"{ModpackDirectory}\"");
            stringBuilder.AppendLine($"LanguageFiles Folder Path: \"{LanguageFilesFolder.FullName}\"");
            stringBuilder.AppendLine($"KJSAssets Folder Path: \"{KjsAssetsFolder}\"");

            return stringBuilder.ToString();
        }
    }
}
