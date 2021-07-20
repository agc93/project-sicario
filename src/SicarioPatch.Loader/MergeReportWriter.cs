using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using SicarioPatch.Core;

namespace SicarioPatch.Loader
{
    public class MergeReportWriter
    {
        private readonly ModParser _parser;

        public MergeReportWriter(ModParser parser) {
            _parser = parser;
        }

        public async Task<FileInfo?> WriteReport(IEnumerable<MergeComponent> mergeComponents, Dictionary<string, string> inputParameters, string? reportPath = null) {
            var reportFile = await BuildReport(mergeComponents, inputParameters);
            if (!string.IsNullOrWhiteSpace(reportPath)) {
                reportFile.MoveTo(reportPath, true);
                return new FileInfo(reportPath);
            }
            return reportFile;
        }

        private async Task<FileInfo> BuildReport(IEnumerable<MergeComponent> mergeComponents,
            Dictionary<string, string> inputParameters) {
            var opts = new JsonSerializerOptions(_parser.Options) {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            var report = new Dictionary<string, object> {[nameof(inputParameters)] = inputParameters};
            foreach (var component in mergeComponents.Where(mc =>
                mc.MergedResources != null && !string.IsNullOrWhiteSpace(mc.Name) && mc.MergedResources.Any())) {
                report.Add(component.Name!, component.MergedResources!);
            }
            var json = JsonSerializer.Serialize(report, opts);
            var file = new FileInfo(Path.GetTempFileName());
            await File.WriteAllTextAsync(file.FullName, json);
            return file;
        }
        
        
    }
}