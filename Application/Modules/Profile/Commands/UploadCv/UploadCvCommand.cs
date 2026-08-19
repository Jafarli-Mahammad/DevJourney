using Application.Modules.Profile.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Profile.Commands.UploadCv
{
    public class UploadCvCommand : IRequest<CvUploadResultDto>
    {
        public IFormFile File { get; set; } = null!;
    }

    public class UploadCvCommandHandler : IRequestHandler<UploadCvCommand, CvUploadResultDto>
    {
        private readonly Application.Common.Interfaces.IFileStorage _fileStorage;

        public UploadCvCommandHandler(Application.Common.Interfaces.IFileStorage fileStorage)
        {
            _fileStorage = fileStorage;
        }

        public async Task<CvUploadResultDto> Handle(UploadCvCommand request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
            {
                throw new Application.Exceptions.BadRequestException("No file was uploaded or the file is empty.");
            }

            var rawExt = System.IO.Path.GetExtension(request.File.FileName);
            var extension = !string.IsNullOrWhiteSpace(rawExt) ? rawExt.ToLowerInvariant() : ".pdf";
            var objectKey = $"{Guid.NewGuid()}{extension}"; // SEC: Random UUID to prevent enumeration/traversal

            using var stream = request.File.OpenReadStream();
            var contentType = !string.IsNullOrWhiteSpace(request.File.ContentType) ? request.File.ContentType : "application/pdf";
            var url = await _fileStorage.UploadFileAsync("cvs", objectKey, stream, contentType, cancellationToken);

            return new CvUploadResultDto { AssetId = Guid.NewGuid(), Url = url };
        }
    }
}
