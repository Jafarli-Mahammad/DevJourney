using Application.Exceptions;
using Application.Repositories.Core;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Certificates.Queries.VerifyCertificate
{
    public class VerifyCertificateQuery : IRequest<object>
    {
        public string CodeOrId { get; set; } = string.Empty;
    }

    public class VerifyCertificateQueryHandler : IRequestHandler<VerifyCertificateQuery, object>
    {
        private readonly ICertificateRepository _certificateRepo;

        public VerifyCertificateQueryHandler(ICertificateRepository certificateRepo)
        {
            _certificateRepo = certificateRepo;
        }

        public async Task<object> Handle(VerifyCertificateQuery request, CancellationToken cancellationToken)
        {
            if (Guid.TryParse(request.CodeOrId, out Guid id))
            {
                var cert = await _certificateRepo.GetAsync(c => c.Id == id, null, cancellationToken);
                if (cert != null)
                {
                    return new { status = "VALID", verificationCode = request.CodeOrId, certificate = new { cert.Title, cert.Description, cert.AssetId } };
                }
            }
            
            throw new NotFoundException("Certificate", request.CodeOrId);
        }
    }
}
