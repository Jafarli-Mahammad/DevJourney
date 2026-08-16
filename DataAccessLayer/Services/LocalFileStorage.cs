using Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessLayer.Services
{
    public class LocalFileStorage : IFileStorage
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LocalFileStorage(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetPhysicalPath(string containerName, string objectKey)
        {
            // SEC: Never allow uploads into the web root (wwwroot). Store them in a restricted volume.
            var uploadsFolder = Path.Combine(_env.ContentRootPath, "App_Data", "uploads", containerName);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            return Path.Combine(uploadsFolder, objectKey);
        }

        public async Task<string> UploadFileAsync(string containerName, string objectKey, Stream fileStream, string contentType, CancellationToken cancellationToken)
        {
            var filePath = GetPhysicalPath(containerName, objectKey);
            using (var fileStreamOutput = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
            {
                await fileStream.CopyToAsync(fileStreamOutput, cancellationToken);
            }
            return await GetFileUrlAsync(containerName, objectKey, cancellationToken);
        }

        public Task<Stream?> DownloadFileAsync(string containerName, string objectKey, CancellationToken cancellationToken)
        {
            var filePath = GetPhysicalPath(containerName, objectKey);
            if (!File.Exists(filePath))
            {
                return Task.FromResult<Stream?>(null);
            }
            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
            return Task.FromResult<Stream?>(stream);
        }

        public Task DeleteFileAsync(string containerName, string objectKey, CancellationToken cancellationToken)
        {
            var filePath = GetPhysicalPath(containerName, objectKey);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            return Task.CompletedTask;
        }

        public Task<string> GetFileUrlAsync(string containerName, string objectKey, CancellationToken cancellationToken)
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            var baseUrl = request != null ? $"{request.Scheme}://{request.Host}" : "";
            var url = $"{baseUrl}/uploads/{containerName}/{objectKey}";
            return Task.FromResult(url);
        }
    }
}
