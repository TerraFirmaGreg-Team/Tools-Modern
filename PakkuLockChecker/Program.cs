using System.Text;
using Common;
using Newtonsoft.Json;

namespace PakkuLockChecker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var modpackFolder = CommonUtil.GetModpackDirectory(Directory.GetCurrentDirectory()).FullName;


            var path = Path.Combine(modpackFolder, "pakku-lock.json");
            if(!File.Exists(path))
            {
                ConsoleLogHelper.WriteLine($"Supplied path of \"{path}\" does not point to a real file.", LogLevel.Error);
                return;
            }

            string json = File.ReadAllText(path);

            var deserializedPakkuLock = JsonConvert.DeserializeObject<DeserializedPakkuLock>(json);
            if(deserializedPakkuLock.Projects == null)
            {
                ConsoleLogHelper.WriteLine($"Supplied path of \"{path}\" is not a pakku-lock.json file", LogLevel.Error);
                return;
            }

            if (deserializedPakkuLock.Projects.Length == 0)
            {
                ConsoleLogHelper.WriteLine($"Supplied path of \"{path}\" is a pakku-lock.json file, but no projects are specified.", LogLevel.Error);
                return;
            }

            List<ProblematicMod> problematicProjects = new List<ProblematicMod>();
            ConsoleLogHelper.WriteLine("PakkuLockChecker start.", LogLevel.Message);
            for (int i = 0; i < deserializedPakkuLock.Projects.Length; i++)
            {
                PakkuProject? pakkuProject = deserializedPakkuLock.Projects[i];

                if(pakkuProject == null)
                {
                    ConsoleLogHelper.WriteLine($"Pakku Project of index {i} is invalid or malformed. This will not be checked.", LogLevel.Error);
                    continue;
                }

                if (string.IsNullOrEmpty(pakkuProject.GetModName))
                {
                    ConsoleLogHelper.WriteLine($"Pakku Project of index {i} has an invalid Slug! This will not be checked.", LogLevel.Error);
                    continue;
                }

                //ConsoleLogHelper.WriteLine($"Checking Pakku Project of index {i} with name {pakkuProject.GetModName}", LogLevel.Info);

                var files = pakkuProject.Files;
                if(files == null)
                {
                    ConsoleLogHelper.WriteLine($"Pakku Project for {pakkuProject.GetModName} has no Files! the array itself is null. This will not be checked.", LogLevel.Error);
                    continue;
                }

                if(files.Length == 0)
                {
                    ConsoleLogHelper.WriteLine($"Pakku Project for {pakkuProject.GetModName} has no File Entries! This will not be checked.", LogLevel.Error);
                    continue;
                }

                int modrinthIndex = -1;
                bool foundModrinth = false;
                int curseforgeIndex = -1;
                bool foundCurseforge = false;
                for (int j = 0; j < files.Length; j++)
                {
                    PakkuFile? file = files[j];
                    if (file.Type == "github")
                    {
                        continue;
                    }

                    foundModrinth |= file.Type == "modrinth";
                    foundCurseforge |= file.Type == "curseforge";

                    if(foundModrinth && modrinthIndex == -1)
                    {
                        modrinthIndex = j;
                    }

                    if(foundCurseforge && curseforgeIndex == -1)
                    {
                        curseforgeIndex = j;
                    }
                }

                if(!foundModrinth)
                {
                    ConsoleLogHelper.WriteLine($"Pakku Project for {pakkuProject.GetModName} does not have Modrinth listed! This may cause issues.", LogLevel.Warning);
                    problematicProjects.Add(new ProblematicMod
                    {
                        modName = pakkuProject.GetModName,
                        uniqueIndex = i,
                        curseforgeInnerIndex = curseforgeIndex,
                        modrinthInnerIndex = modrinthIndex,
                        problemType = ProblematicModType.MissingModrinth
                    });
                    continue;
                }

                if(!foundCurseforge)
                {
                    ConsoleLogHelper.WriteLine($"Pakku Project for {pakkuProject.GetModName} does not have Curseforge listed! This may cause issues.", LogLevel.Warning);
                    problematicProjects.Add(new ProblematicMod
                    {
                        modName = pakkuProject.GetModName,
                        uniqueIndex = i,
                        curseforgeInnerIndex = curseforgeIndex,
                        modrinthInnerIndex = modrinthIndex,
                        problemType = ProblematicModType.MissingCurseforge
                    });
                    continue;
                }

                if(foundModrinth && foundCurseforge)
                {
                    PakkuFile modrinthFile = files[modrinthIndex];
                    PakkuFile curseforgeFile = files[curseforgeIndex];

                    if(modrinthFile.File_Name != curseforgeFile.File_Name)
                    {
                        ConsoleLogHelper.WriteLine($"Pakku Project for {pakkuProject.GetModName} has both Modrinth and Curseforge listed, but their file names differ! this may be a versioning issue.", LogLevel.Warning);
                        problematicProjects.Add(new ProblematicMod
                        {
                            modName = pakkuProject.GetModName,
                            uniqueIndex = i,
                            curseforgeInnerIndex = curseforgeIndex,
                            modrinthInnerIndex = modrinthIndex,
                            problemType = ProblematicModType.MismatchFileNames
                        });
                    }
                }
            }


            StringBuilder finalizer = new StringBuilder();
            finalizer.AppendLine("The following mods have been spotted with potential issues. More info above.");
            foreach(var index in problematicProjects)
            {
                finalizer.AppendLine(index.ToString(deserializedPakkuLock));
            }
            ConsoleLogHelper.WriteLine(finalizer.ToString(), LogLevel.Message);

            ConsoleLogHelper.WriteLine("Checking complete! Press any key to exit...", LogLevel.Info);
            Console.ReadKey();
        }

        [Serializable]
        private class DeserializedPakkuLock
        {
            [JsonProperty("projects")]
            public PakkuProject[]? Projects { get; set; }
        }

        [Serializable]
        private class PakkuProject
        {
            public string? GetModName
            {
                get
                {
                    return Slug?.Curseforge ?? Slug?.Modrinth ?? Slug?.Github;
                }
            }
            [JsonProperty("slug")]
            public Slug? Slug { get; set; }

            [JsonProperty("files")]
            public PakkuFile[]? Files { get; set; }
        }

        [Serializable]
        private class Slug
        {
            [JsonProperty("curseforge")]
            public string? Curseforge { get; set; }
            [JsonProperty("modrinth")]
            public string? Modrinth { get; set; }

            [JsonProperty("github")]
            public string? Github { get; set; }
        }

        [Serializable]
        private class PakkuFile
        {
            [JsonProperty("type")]
            public string? Type { get; set; }
            [JsonProperty("file_name")]
            public string? File_Name { get; set; }
        }

        private struct ProblematicMod
        {
            public string modName;
            public int uniqueIndex;
            public int? curseforgeInnerIndex;
            public int? modrinthInnerIndex;

            public ProblematicModType problemType;

            public override string ToString()
            {
                return GetBuilderForToString().ToString();
            }

            public string ToString(DeserializedPakkuLock lockFile)
            {
                StringBuilder builder = GetBuilderForToString();

                if(problemType == ProblematicModType.MismatchFileNames)
                {
                    builder.AppendLine("    * Curseforge file name: " + lockFile.Projects[uniqueIndex].Files[curseforgeInnerIndex.Value].File_Name);
                    builder.AppendLine("    * Modrinth file name: " + lockFile.Projects[uniqueIndex].Files[modrinthInnerIndex.Value].File_Name);
                }
                return builder.ToString();
            }

            private StringBuilder GetBuilderForToString()
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine($"# Mod Name: {modName}({uniqueIndex});");
                builder.AppendLine($"* Issue Type: {problemType}.");
                return builder;
            }
        }

        private enum ProblematicModType
        {
            MissingModrinth,
            MissingCurseforge,
            MismatchFileNames,
        }
    }
}
