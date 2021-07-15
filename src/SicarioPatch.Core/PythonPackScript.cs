using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BuildEngine;
using BuildEngine.Scripts;
using ExecEngine;
using HexPatch.Build;

namespace SicarioPatch.Core
{
    public class PythonPackScript : ScriptRunner, IModBuilder
    {
        public PythonPackScript() : base(new RemoteScript("u4pak.py", "https://raw.githubusercontent.com/panzi/u4pak/8ea81ea019c0fea4e3cbe29720ed9a94e3875e0b/u4pak.py") {FileSourceHash = "6e5858b7fee1ccb304218b98886b1a1e"}, "python", "u4pak.py")
        {
        }

        protected override IEnumerable<string> GetBuildArgs(FileInfo scriptPath, string targetFileName) {
            var gameDir = Path.GetFileName(Directory.GetDirectories(scriptPath.Directory.FullName).First());
            var args = new List<string> {"pack", targetFileName.ToArgument(),  gameDir.ToArgument()};
            return args;
        }

        public async Task<(bool Success, FileSystemInfo Output)> RunBuildAsync(IBuildContext buildContext, string targetFileName) {
            if (buildContext is not DirectoryBuildContext ctx) {
                throw new InvalidOperationException("Unsupported build context!");
            }
            return await this.RunBuildAsync(ctx.WorkingDirectory.FullName, targetFileName);
        }
    }
}