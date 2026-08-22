using Application.Repositories.Core;
using Application.Services;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Repositories;


namespace Application.Modules.Certificates.Queries.GetPartnerIssuedCertificates
{
    public class GetPartnerIssuedCertificatesQuery : IRequest<object>
    {
    }

    public class GetPartnerIssuedCertificatesQueryHandler : IRequestHandler<GetPartnerIssuedCertificatesQuery, object>
    {
        private readonly ICertificateRepository _certificateRepo;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPartnerProfileRepository _partnerProfileRepo;

        public GetPartnerIssuedCertificatesQueryHandler(ICertificateRepository certificateRepo, ICurrentUserService currentUserService, IPartnerProfileRepository partnerProfileRepo)
        {
            _certificateRepo = certificateRepo;
            _currentUserService = currentUserService;
            _partnerProfileRepo = partnerProfileRepo;
        }

        public async Task<object> Handle(GetPartnerIssuedCertificatesQuery request, CancellationToken cancellationToken)
        {
            var partners = await _partnerProfileRepo.GetAllAsync(p => p.ApplicationUserId == _currentUserService.UserId, cancellationToken);
            var partner = partners.FirstOrDefault();
            
            if (partner == null) return Array.Empty<object>();

            var certs = await _certificateRepo.GetAllAsync(c => c.IssuedByPartnerId == partner.Id, cancellationToken);
            return certs.Select(c => new 
            {
                c.Id,
                c.Title,
                c.Description,
                c.PendingEmail,
                c.UserId,
                c.AssetId
            }).ToList();
        }
    }
}
