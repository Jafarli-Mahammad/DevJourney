using MediatR;
using Microsoft.AspNetCore.Http;
using System;

namespace Application.Modules.Certificates.Commands.UploadCertificate
{
    public class UploadCertificateCommand : IRequest<Guid>
    {
        public IFormFile CertificateFile { get; set; } = null!;
        public string StudentEmail { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public Guid? PartnerId { get; set; }
    }
}
