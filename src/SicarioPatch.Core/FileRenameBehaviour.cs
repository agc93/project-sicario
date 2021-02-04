using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BuildEngine;
using MediatR;

namespace SicarioPatch.Core
{
    public class FileRenameBehaviour : IPipelineBehavior<PatchRequest, FileInfo>
    {
        public async Task<FileInfo> Handle(PatchRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<FileInfo> next)
        {
            var result = await next();
            if (!string.IsNullOrWhiteSpace(request?.Name))
            {
                var targetFileName = Path.GetFileNameWithoutExtension(request.Name.MakeSafe());
                var targetFilePath = Path.Combine(result.Directory.FullName, targetFileName + result.Extension);
                result.MoveTo(targetFilePath, true);
                return new FileInfo(targetFilePath);
            }
            return result;
        }
    }
}