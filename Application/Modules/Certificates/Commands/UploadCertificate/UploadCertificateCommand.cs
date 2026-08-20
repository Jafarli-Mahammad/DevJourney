using MediatR;
using Microsoft.AspNetCore.Http;
using System;

namespace Application.Modules.Certificates.Commands.UploadCertificate
{
    public class UploadCertificateCommand : IRequest<Guid>
    {
        public IFormFile? CertificateFile { get; set; }
        public IFormFile? Certificate { get; set; }
        public IFormFile? File { get; set; }

        public string? StudentEmail { get; set; }
        public string? Email { get; set; }

        public string? Title { get; set; }
        public string? CompetitionTitle { get; set; }

        public string? Description { get; set; }
        public string? Desc { get; set; }

        public Guid? PartnerId { get; set; }
        public Guid? CompetitionId { get; set; }
    }
}
