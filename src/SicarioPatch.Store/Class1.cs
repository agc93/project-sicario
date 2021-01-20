using System;
using System.Collections.Generic;
using System.IO;
using FileStorEngine;
using FileStorEngine.Services;
using HexPatch;
using HexPatch.Build;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SicarioPatch.Store
{
    public static class StoreExtensions
    {
        public static IServiceCollection AddStoreServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddFileStore(c =>
            {
                c.AddLocalIndex().AddLocalStorage().ConfigureOptions(config.GetSection("Store"));
            });
            services.AddSingleton<StoreModLoader>();
            return services;
        }
    }

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

    public class StoreModLoader
    {
    }
}
