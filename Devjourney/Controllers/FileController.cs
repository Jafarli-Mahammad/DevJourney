using Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Devjourney.Controllers
{
    [ApiController]
    [Route("uploads")]
    public class FileController : ControllerBase
    {
        private readonly IFileStorage _fileStorage;

        public FileController(IFileStorage fileStorage)
        {
            _fileStorage = fileStorage;
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

            return File(stream, contentType);
        }
    }
}
