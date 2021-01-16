using System.IO;
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

        public async Task<string> GetScriptPath()
        {
            var fi = new FileInfo(ScriptFilePath);
            if (fi.Exists && fi.Length > 0)
            {
                return fi.FullName;
            }
            else
            {
                await DownloadScript("https://raw.githubusercontent.com/panzi/u4pak/master/u4pak.py");
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
    }
}