using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BuildEngine.Scripts;
using Microsoft.Extensions.Logging;

namespace SicarioPatch.Core
{
    public class PythonScriptDownloadService : ScriptDownloadBase, IScriptService
    {
        public PythonScriptDownloadService(ILogger<PythonScriptDownloadService> logger, BuildEngine.IAppInfoProvider infoProvider) : base("u4pak.py", logger, infoProvider)
        {
        }
        
        private (string FileUri, string Hash) GetSourceScript() {
            return ("https://raw.githubusercontent.com/panzi/u4pak/8ea81ea019c0fea4e3cbe29720ed9a94e3875e0b/u4pak.py",
                "6e5858b7fee1ccb304218b98886b1a1e");
        }

        public async Task<string> GetScriptPath() {
            var src = GetSourceScript();
            var fi = new FileInfo(ScriptFilePath);
            if (fi.Exists && fi.Length > 0 && CalculateMD5(fi) == src.Hash)
            {
                return fi.FullName;
            }
            else
            {
                await DownloadScript(src.FileUri);
                return fi.FullName;
            }
        }

        public async Task<BuildScript> GetScriptContext(string targetPath)
        {
            var sourceFile = await GetScriptPath();
            var targetFile = Path.Combine(targetPath, "u4pak.py");
            File.Copy(sourceFile, targetFile, true);
            return new PythonPackScript(targetFile);
        }
        
        internal static string CalculateMD5(FileInfo fi)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = fi.OpenRead())
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}