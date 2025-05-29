namespace Common
{
	public static class CommonUtil
	{
		public static string GetModpackDirectory()
		{
			var workingDir = Directory.GetCurrentDirectory();

			var toolsIndex = workingDir.IndexOf("Tools-Modern");
			if (toolsIndex == -1)
			{
				throw new DirectoryNotFoundException("Failed to find \"Tools-Modern\" directory.");
			}

			var teamDir = workingDir.Substring(0, toolsIndex);

			return Path.Combine(teamDir, "Modpack-Modern");
		}

		public static string GetKJSAssetsFolder(string modpackFolder)
		{
			string kjsAssetsFolder = Path.Combine(modpackFolder, "kubejs", "assets");
			if (!Directory.Exists(kjsAssetsFolder))
			{
				throw new DirectoryNotFoundException($"The \"assets\" folder was not found in {kjsAssetsFolder}");
			}

			return kjsAssetsFolder;
		}
	}
}
