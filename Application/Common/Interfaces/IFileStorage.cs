using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IFileStorage
    {
        Task<string> UploadFileAsync(string containerName, string objectKey, Stream fileStream, string contentType, CancellationToken cancellationToken);
        Task<Stream?> DownloadFileAsync(string containerName, string objectKey, CancellationToken cancellationToken);
        Task DeleteFileAsync(string containerName, string objectKey, CancellationToken cancellationToken);
        Task<string> GetFileUrlAsync(string containerName, string objectKey, CancellationToken cancellationToken);
    }
}
