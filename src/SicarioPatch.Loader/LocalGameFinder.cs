using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SicarioPatch.Integration;

namespace SicarioPatch.Loader
{
    public class LocalGameFinder : IGameSource
    {
        private string _rootPath;
        public LocalGameFinder() : this(AppContext.BaseDirectory) {
            
        }

        public LocalGameFinder(string rootPath) {
            
        }
        private static Dictionary<string, Func<DirectoryInfo, DirectoryInfo?>> Candidates => new() {
            ["Project Wingman"] = di => di.GetFiles("ProjectWingman.exe").Any() ? di : null,
            ["ProjectWingman"] = di => di.Parent!.GetFiles("ProjectWingman.exe").Any() ? di.Parent : null,
            ["Paks"] = di => di.Parent?.Parent?.Parent,
            ["~mods"] = di => di.Parent?.Parent?.Parent?.Parent
        };
        public string? GetGamePath() {
            var di = new DirectoryInfo(_rootPath);
            if (di.Exists && Candidates.ContainsKey(di.Name)) {
                var targetDir = Candidates[di.Name].Invoke(di);
                return targetDir?.FullName;
            }
            return null;
        }

        public string? GetGamePakPath() {
            var dir = GetGamePath();
            if (dir != null) {
                var pakFilePath = Path.Join(dir, "ProjectWingman", "Content", "Paks",
                    "ProjectWingman-WindowsNoEditor.pak");
                return File.Exists(pakFilePath) ? pakFilePath : null;
            }
            return null;
        }
    }
}