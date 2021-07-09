using System;
using System.Collections.Generic;
using System.IO;
using SicarioPatch.Integration;

namespace SicarioPatch.Loader
{
    public class LocalGameFinder : IGameSource
    {
        private static Dictionary<string, Func<DirectoryInfo, DirectoryInfo?>> Candidates => new() {
            ["Paks"] = di => di.Parent?.Parent?.Parent,
            ["~mods"] = di => di.Parent?.Parent?.Parent?.Parent
        };
        public string? GetGamePath() {
            var di = new DirectoryInfo(AppContext.BaseDirectory);
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