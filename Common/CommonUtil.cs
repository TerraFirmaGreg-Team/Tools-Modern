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

			var modpackDir = Path.Combine(teamDir, "Modpack-Modern");
			if (Directory.Exists(modpackDir))
			{
				return modpackDir;
			}
			else
			{
				// Try an alternative
				modpackDir = Path.Combine(teamDir, "TerraFirmaGreg-Modern");
				if (Directory.Exists(modpackDir))
				{
					return modpackDir;
				}
				else
				{
					throw new DirectoryNotFoundException("Failed to find \"Modpack-Modern\" or \"TerraFirmaGreg-Modern\" directory.");
				}
			}
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

		public static string GetKJSDataFolder(string modpackFolder)
		{
			string kjsDataFolder = Path.Combine(modpackFolder, "kubejs", "data");
			if (!Directory.Exists(kjsDataFolder))
			{
				throw new DirectoryNotFoundException($"The \"data\" folder was not found in {kjsDataFolder}");
			}

			return kjsDataFolder;
		}
	}
}
