using Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Devjourney.Controllers
{
    [ApiController]
    [Route("uploads")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class FileController : ControllerBase
    {
        private readonly IFileStorage _fileStorage;

        public FileController(IFileStorage fileStorage)
        {
            _fileStorage = fileStorage;
        }

        [HttpPost("image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var ext = Path.GetExtension(file.FileName).ToLower();
            if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".svg")
            {
                return BadRequest("Invalid image format. Allowed formats are png, jpg, jpeg, svg.");
            }

            var containerName = "images";
            var objectKey = $"{Guid.NewGuid()}{ext}";

            using var stream = file.OpenReadStream();
            await _fileStorage.UploadFileAsync(containerName, objectKey, stream, file.ContentType, cancellationToken);
            
            var url = await _fileStorage.GetFileUrlAsync(containerName, objectKey, cancellationToken);
            return Ok(new { success = true, data = new { url = url } });
        }

        [HttpPost("document")]
        [HttpPost("file")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadDocument(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var ext = Path.GetExtension(file.FileName).ToLower();
            if (ext != ".pdf" && ext != ".docx" && ext != ".doc")
            {
                return BadRequest("Invalid document format. Allowed formats are pdf, docx, doc.");
            }

            var containerName = "documents";
            var objectKey = $"{Guid.NewGuid()}{ext}";

            using var stream = file.OpenReadStream();
            await _fileStorage.UploadFileAsync(containerName, objectKey, stream, file.ContentType, cancellationToken);
            
            var url = await _fileStorage.GetFileUrlAsync(containerName, objectKey, cancellationToken);
            return Ok(new { success = true, data = new { url = url } });
        }

        [HttpGet("{containerName}/{objectKey}")]
        public async Task<IActionResult> DownloadFile(string containerName, string objectKey, CancellationToken cancellationToken)
        {
            // SEC: Prevent directory traversal
            if (objectKey.Contains("..") || objectKey.Contains("/") || objectKey.Contains("\\"))
            {
                return BadRequest("Invalid object key.");
            }

            var stream = await _fileStorage.DownloadFileAsync(containerName, objectKey, cancellationToken);
            if (stream == null)
            {
                return NotFound();
            }

            // SEC: Enforce Content-Disposition: attachment to prevent XSS (so the browser downloads instead of executing HTML/SVG/JS inline)
            Response.Headers.Append("Content-Disposition", $"attachment; filename=\"{objectKey}\"");

            var contentType = "application/octet-stream";
            if (objectKey.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                contentType = "application/pdf";
            }
            else if (objectKey.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
            {
                contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            }
            else if (objectKey.EndsWith(".doc", StringComparison.OrdinalIgnoreCase))
            {
                contentType = "application/msword";
            }
            else if (objectKey.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || objectKey.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                contentType = "image/jpeg";
            }
            else if (objectKey.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                contentType = "image/png";
            }
            else if (objectKey.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
            {
                contentType = "image/svg+xml";
            }

            return File(stream, contentType);
        }
    }
}
