using System.IO;

namespace SicarioPatch.Store
{
    public class FileStore
    {
        private IFileUploadService _uploadService;
        private IFileIndexService _index;
        
        public FileStore(IFileUploadService uploadService, IFileIndexService indexService)
        {
            _uploadService = uploadService;
            _index = indexService;
        }

        public FileInfo GetFile(string name)
        {
            var fileRef = ShortCode.TryParse(name, out var c)
                ? _index.GetFile(c)
                : _index.GetFileByName(c);
            return new FileInfo(Path.Combine(_uploadService.RootPath.FullName, fileRef.Location));
        }

        public ShortCode SaveFile(string rawText, string fileName)
        {
            var file = _uploadService.Save(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(rawText), false), fileName);
            var imgCode = _index.AddFile(System.IO.Path.GetRelativePath(_uploadService.RootPath.FullName, file.FullName), fileName);

            _index.AddFile(imgCode);
            return imgCode;
        }
    }
}