using System.Diagnostics;
using System.Reflection;
using BuildEngine;

namespace SicarioPatch.Core
{
    public class AppInfoProvider : IAppInfoProvider
    {
        private readonly string _version;

        public AppInfoProvider()
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName();
            var version = assemblyName?.Version?.ToString();
            _version = version ?? "0.1.0";
        }
        public (string Name, string Version) GetAppInfo()
        {
            return ("SicarioPatch", _version);
        }
    }
}