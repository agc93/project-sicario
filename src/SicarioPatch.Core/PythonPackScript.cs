using System.Collections.Generic;
using System.IO;
using System.Linq;
using BuildEngine.Scripts;
using ExecEngine;

namespace SicarioPatch.Core
{
    public class PythonPackScript : BuildScript
    {
        public PythonPackScript(string targetPath, params string[] defaultArgs) : base(targetPath, defaultArgs)
        {
        }

        public override (bool Success, FileSystemInfo Output) RunBuild(string targetFileName)
        {
            var gameDir = Path.GetFileName(Directory.GetDirectories(WorkingDirectory).First());
            var args = new List<string> {"pack", targetFileName.ToArgument(),  gameDir.ToArgument()};
            var runner = new CommandRunner("python", "u4pak.py").SetWorkingDirectory(WorkingDirectory);
            var output = runner.RunCommand(args);
            return (output.ExitCode == 0, new FileInfo(Path.IsPathRooted(targetFileName) ? targetFileName : Path.Combine(WorkingDirectory, targetFileName)));
        }
    }
}