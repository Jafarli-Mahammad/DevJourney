using MediatR;
using System.Collections.Generic;

namespace Application.Modules.Certificates.Queries.GetMyCertificates
{
    public class GetMyCertificatesQuery : IRequest<List<CertificateDto>>
    {
    }

    public class CertificateDto
    {
        public System.Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? AssetId { get; set; }
        public string? PendingEmail { get; set; }
    }
}
