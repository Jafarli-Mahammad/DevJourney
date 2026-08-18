using Application.Repositories.Core;
using Application.Services;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Certificates.Queries.GetMyCertificates
{
    public class GetMyCertificatesQueryHandler : IRequestHandler<GetMyCertificatesQuery, List<CertificateDto>>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetMyCertificatesQueryHandler(ICertificateRepository certificateRepository, ICurrentUserService currentUserService)
        {
            _certificateRepository = certificateRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<CertificateDto>> Handle(GetMyCertificatesQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            
            // Note: Since IAsyncRepository.GetAsync might only return one or we might need GetAllAsync
            var certificates = await _certificateRepository.GetAllAsync(c => c.UserId == userId, cancellationToken);
            
            return certificates.Select(c => new CertificateDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                AssetId = c.AssetId,
                PendingEmail = c.PendingEmail
            }).ToList();
        }
    }
}
