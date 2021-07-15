using System;
using System.Collections.Generic;
using System.IO;
using ExecEngine;

namespace SicarioPatch.Integration
{
    public class GameLauncher
    {
        private readonly CommandRunner _runner;

        public GameLauncher(string installPath) {
            _runner = GetRunner(installPath);
        }

        public GameLauncher(IEnumerable<IGameSource> gameSources) {
            var installPath = gameSources.GetGamePath() ?? Environment.CurrentDirectory;
            _runner = GetRunner(installPath);
        }

        private static CommandRunner GetRunner(string installPath) {
            var gamePath = Path.Combine(installPath, "ProjectWingman.exe");
            return new CommandRunner(gamePath) {
                RunDetached = true,
                Name = "ProjectWingman-Game"
            };
        }

        public void RunGame() {
            var _ =_runner.RunCommand();
        }
    }
}