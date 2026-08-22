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
        private readonly ICurrentUserService _currentUserService;
        private readonly IPartnerProfileRepository _partnerProfileRepository;

        public UploadCertificateCommandHandler(
            ICertificateRepository certificateRepository,
            IUnitOfWork unitOfWork,
            IFileStorage fileStorage,
            IAuthService authService,
            ICurrentUserService currentUserService,
            IPartnerProfileRepository partnerProfileRepository)
        {
            _certificateRepository = certificateRepository;
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
            _authService = authService;
            _currentUserService = currentUserService;
            _partnerProfileRepository = partnerProfileRepository;
        }

        public async Task<Guid> Handle(UploadCertificateCommand request, CancellationToken cancellationToken)
        {
            var file = request.CertificateFile ?? request.Certificate ?? request.File;
            if (file == null || file.Length == 0)
            {
                throw new Application.Exceptions.BadRequestException("Certificate file is required.");
            }

            var studentEmail = request.StudentEmail ?? request.Email;
            if (string.IsNullOrWhiteSpace(studentEmail))
            {
                throw new Application.Exceptions.BadRequestException("Student email is required.");
            }

            var title = request.Title ?? request.CompetitionTitle ?? "Certificate of Achievement";
            var description = request.Description ?? request.Desc;

            // Resolve PartnerId if not explicitly provided
            var partnerId = request.PartnerId;
            if (!partnerId.HasValue && _currentUserService.IsAuthenticated && _currentUserService.UserId != Guid.Empty)
            {
                var partners = await _partnerProfileRepository.GetAllAsync(p => p.ApplicationUserId == _currentUserService.UserId, cancellationToken);
                partnerId = partners.FirstOrDefault()?.Id;
            }

            // 1. Upload the certificate file
            var fileExtension = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var containerName = "certificates"; // Store in a specific container/folder

            using var stream = file.OpenReadStream();
            await _fileStorage.UploadFileAsync(containerName, uniqueFileName, stream, file.ContentType, cancellationToken);
            var assetId = $"{containerName}/{uniqueFileName}";

            // 2. Check if the user exists
            var user = await _authService.GetUserInfoByEmailAsync(studentEmail.Trim());

            // 3. Create the Certificate entity
            var certificate = new Certificate
            {
                UserId = user?.UserId,
                PendingEmail = user == null ? studentEmail.Trim() : null,
                IssuedByPartnerId = partnerId,
                Title = title.Trim(),
                Description = description?.Trim(),
                AssetId = assetId
            };

            await _certificateRepository.AddAsync(certificate);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return certificate.Id;
        }
    }
}
