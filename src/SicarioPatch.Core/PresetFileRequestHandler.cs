using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace SicarioPatch.Core
{
    public class PresetFileRequestHandler : IRequestHandler<PresetFileRequest, FileInfo>
    {
        private readonly ModParser _parser;

        public PresetFileRequestHandler(ModParser parser) {
            _parser = parser;
        }
        public async Task<FileInfo> Handle(PresetFileRequest request, CancellationToken cancellationToken) {
            var preset = new WingmanPreset() {
                Mods = request.Mods,
                ModParameters = request.TemplateInputs,
                Version = request.Version ?? 1
            };
            var tempFile = Path.GetTempFileName();
            var opts = new JsonSerializerOptions(_parser.Options) {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                IgnoreNullValues = true
            };
            var json = JsonSerializer.Serialize(preset, opts);
            await File.WriteAllTextAsync(tempFile, json, Encoding.ASCII, cancellationToken);
            var name = request.PresetName ?? $"preset-{System.DateTime.UtcNow.Ticks}";
            
            var finalTarget = Path.Combine(Path.GetTempPath(), $"{Path.ChangeExtension(name, ".dtp")}");
            var fi = new FileInfo(finalTarget);
            File.Move(tempFile, fi.FullName);
            return fi;
        }
    }
}