using BuildEngine;

namespace SicarioPatch.Core
{
    public class AppInfoProvider : IAppInfoProvider
    {
        public (string Name, string Version) GetAppInfo()
        {
            return ("SicarioPatch", "0.1.0");
        }
    }
}