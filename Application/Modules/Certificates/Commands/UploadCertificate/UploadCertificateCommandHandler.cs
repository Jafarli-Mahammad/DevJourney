using Application.Common.Interfaces;
using Application.Repositories;
using Application.Repositories.Core;
using Application.Services;
using Domain.Models.Entities.Core;
using MediatR;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Certificates.Commands.UploadCertificate
{
    public class UploadCertificateCommandHandler : IRequestHandler<UploadCertificateCommand, Guid>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorage _fileStorage;
        private readonly IAuthService _authService;

        public UploadCertificateCommandHandler(
            ICertificateRepository certificateRepository,
            IUnitOfWork unitOfWork,
            IFileStorage fileStorage,
            IAuthService authService)
        {
            _certificateRepository = certificateRepository;
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
            _authService = authService;
        }

        public async Task<Guid> Handle(UploadCertificateCommand request, CancellationToken cancellationToken)
        {
            // 1. Upload the certificate SVG file
            var fileExtension = Path.GetExtension(request.CertificateFile.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var containerName = "certificates"; // Store in a specific container/folder

            using var stream = request.CertificateFile.OpenReadStream();
            await _fileStorage.UploadFileAsync(containerName, uniqueFileName, stream, request.CertificateFile.ContentType, cancellationToken);
            var assetId = $"{containerName}/{uniqueFileName}";

            // 2. Check if the user exists
            var user = await _authService.GetUserInfoByEmailAsync(request.StudentEmail);

            // 3. Create the Certificate entity
            var certificate = new Certificate
            {
                UserId = user?.UserId,
                PendingEmail = user == null ? request.StudentEmail : null,
                IssuedByPartnerId = request.PartnerId,
                Title = request.Title,
                Description = request.Description,
                AssetId = assetId
            };

            await _certificateRepository.AddAsync(certificate);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return certificate.Id;
        }
    }
}
