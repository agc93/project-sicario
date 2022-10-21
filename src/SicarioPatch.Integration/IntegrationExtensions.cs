using System.Collections.Generic;
using System.IO;
using System.Linq;
using SemanticVersioning;
using SicarioPatch.Core;
using SicarioPatch.Engine;
using UnPak.Core;

namespace SicarioPatch.Integration
{
    public static class IntegrationExtensions
    {
        public static string? GetGamePath(this IEnumerable<IGameSource> sources) {
            return sources.Select(gs => gs.GetGamePath())
                .FirstOrDefault(gp => !string.IsNullOrWhiteSpace(gp));
        }

        public static string? GetGamePakPath(this IEnumerable<IGameSource> sources) {
            var pakPath = sources.Select(gs => gs.GetGamePakPath())
                .FirstOrDefault(gp => !string.IsNullOrWhiteSpace(gp));
            return pakPath;
        }

        public static string GetVirtualPath(this Record r, PakFile pakFile) {
            return Path.Join(pakFile.MountPoint, r.FileName)
                .Replace(string.Join("/", new[] {"..", "..", ".."}), string.Empty).TrimStart('/');
        }

        internal static string TrimPathTo(this string path, string pathSegment, string separator = "/") {
            var finalParts = path.Split(separator).SkipWhile(s => !string.Equals(s, pathSegment)).ToList();
            return finalParts.Any() ? string.Join("/", finalParts) : path;
        }

        public static bool IsSupportedBy(this WingmanPreset preset, IEngineInfoProvider engineInfo) {
            var presetRequested = preset.EngineVersion;
            var engineVersion = engineInfo.GetEngineVersion();
            if (!string.IsNullOrWhiteSpace(presetRequested) && !string.IsNullOrWhiteSpace(engineVersion)) {
                if (Version.TryParse(engineVersion, true, out var engineVersionInfo) &&
                    Version.TryParse(presetRequested, true, out var presetVersionInfo)) {
                    var supported = engineVersionInfo == Version.Parse("0.0.0") || engineVersionInfo >= presetVersionInfo;
                    return supported;
                }
                return false;
            }

            return true;
        }
    }
}