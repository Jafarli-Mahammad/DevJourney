using Microsoft.AspNetCore.Mvc;
using System;

using Application.Modules.Certificates.Commands.UploadCertificate;
using Application.Modules.Certificates.Queries.GetMyCertificates;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using Application.Repositories.Core;
using Application.Repositories;
using Domain.Models.Entities.Core;

namespace Devjourney.Controllers
{
    [ApiController]
    [Route("api/certificates")]
    [Produces("application/json", "application/problem+json")]
    public class CertificatesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICertificateRepository _certificateRepository;
        private readonly IStudentProfileRepository _studentProfileRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CertificatesController(IMediator mediator, ICertificateRepository certificateRepository, IStudentProfileRepository studentProfileRepository, IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _certificateRepository = certificateRepository;
            _studentProfileRepository = studentProfileRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCertificates()
        {
            var data = await _mediator.Send(new GetMyCertificatesQuery());
            return Ok(new { success = true, data });
        }

        [HttpPost("upload")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadCertificate([FromForm] UploadCertificateCommand command)
        {
            var certificateId = await _mediator.Send(command);
            return Ok(new { success = true, certificateId });
        }

        [HttpPost("seed-mock")]
        [AllowAnonymous]
        public async Task<IActionResult> SeedMockCertificates()
        {
            // Get some students to attach mock certificates to
            var students = await _studentProfileRepository.GetAllAsync(s => true);
            var studentList = System.Linq.Enumerable.ToList(students);
            
            if (studentList.Count == 0) return BadRequest("No students found to seed.");

            foreach (var student in studentList)
            {
                // Check if they already have certificates to avoid duplicates
                var existing = await _certificateRepository.GetAllAsync(c => c.UserId == student.ApplicationUserId);
                if (System.Linq.Enumerable.Any(existing)) continue;

                var mockCert1 = new Certificate
                {
                    UserId = student.ApplicationUserId,
                    Title = "1st Place - SmartSolutions Hackathon",
                    Description = "Awarded for creating an outstanding MVP",
                    AssetId = "certificates/winner_certificate.jpg"
                };

                var mockCert2 = new Certificate
                {
                    UserId = student.ApplicationUserId,
                    Title = "Participation - DevJourney Startup Days",
                    Description = "Successfully completed the 48-hour startup challenge",
                    AssetId = "certificates/participant_certificate.jpg"
                };

                await _certificateRepository.AddAsync(mockCert1);
                await _certificateRepository.AddAsync(mockCert2);
            }

            await _unitOfWork.SaveChangesAsync();

            return Ok(new { success = true, message = $"Seeded 2 certificates for {studentList.Count} students." });
        }
    }
}
