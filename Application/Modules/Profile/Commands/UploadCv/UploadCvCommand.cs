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
            var extension = System.IO.Path.GetExtension(request.File.FileName);
            var objectKey = $"{Guid.NewGuid()}{extension}"; // SEC: Random UUID to prevent enumeration/traversal

            using var stream = request.File.OpenReadStream();
            var url = await _fileStorage.UploadFileAsync("cvs", objectKey, stream, request.File.ContentType, cancellationToken);

            return new CvUploadResultDto { AssetId = Guid.NewGuid(), Url = url };
        }
    }
}
