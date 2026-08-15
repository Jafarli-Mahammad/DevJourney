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
        public Task<CvUploadResultDto> Handle(UploadCvCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new CvUploadResultDto { AssetId = Guid.NewGuid(), Url = "https://mock.storage.com/cv.pdf" });
        }
    }
}
