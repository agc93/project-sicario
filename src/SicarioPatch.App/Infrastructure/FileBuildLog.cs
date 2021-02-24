using System;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SicarioPatch.Core;

namespace SicarioPatch.App.Infrastructure
{
    public class FileBuildLog : IBuildLog
    {
        private readonly string _logPath;
        
        private readonly JsonSerializerOptions _jsonOpts = new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Converters =
            {
                new System.Text.Json.Serialization.JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

        private ILogger<FileBuildLog> _logger;

        public FileBuildLog(IConfiguration config, ILogger<FileBuildLog> logger)
        {
            _logPath = config.GetValue<string>("BuildLogPath", null);
            _logger = logger;
        }


        public void SaveRequest(PatchRequestSummary summary)
        {
            if (_logPath != null)
            {
                try
                {
                    var json = JsonSerializer.Serialize(summary, _jsonOpts);
                    var targetPath = Path.Combine(_logPath, $"{summary.Id}.json");
                    File.WriteAllText(targetPath, json);
                }
                catch (Exception e)
                {
                    _logger?.LogWarning($"Failed to write summary for {summary.Id}! {e.Message}");
                }
            }
        }
    }
}