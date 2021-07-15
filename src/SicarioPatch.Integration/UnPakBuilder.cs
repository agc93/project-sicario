using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BuildEngine;
using HexPatch.Build;
using UnPak.Core;

namespace SicarioPatch.Integration
{
    public class UnPakBuilder : IModBuilder
    {
        private readonly PakFileProvider _pakFileProvider;

        public UnPakBuilder(PakFileProvider pakFileProvider) {
            _pakFileProvider = pakFileProvider;
        }
        public Task<(bool Success, FileSystemInfo Output)> RunBuildAsync(IBuildContext buildContext, string targetFileName) {
            if (buildContext is not DirectoryBuildContext ctx) {
                throw new InvalidOperationException("Unsupported build context!");
            }
            var writer = _pakFileProvider.GetWriter();
            var gameDir = new DirectoryInfo(Directory.GetDirectories(ctx.WorkingDirectory.FullName).First());
            var targetBuildPath = Path.IsPathRooted(targetFileName) ? targetFileName : Path.Combine(ctx.WorkingDirectory.FullName, targetFileName);
            var fi = writer.BuildFromDirectory(gameDir, new FileInfo(targetBuildPath));
            return Task.FromResult((fi.Exists, fi as FileSystemInfo));
        }
    }
}