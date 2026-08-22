using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Modules.Certificates.Commands.BulkIssueCertificates;
using Application.Modules.Certificates.Queries.GetPartnerIssuedCertificates;
using Application.Modules.Certificates.Queries.VerifyCertificate;
using Application.Repositories;
using Application.Repositories.Core;
using Devjourney.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DevJourney.Tests.Controllers
{
    public class CertificatesControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ICertificateRepository> _certRepoMock;
        private readonly Mock<IStudentProfileRepository> _studentRepoMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly CertificatesController _controller;

        public CertificatesControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _certRepoMock = new Mock<ICertificateRepository>();
            _studentRepoMock = new Mock<IStudentProfileRepository>();
            _uowMock = new Mock<IUnitOfWork>();

            _controller = new CertificatesController(_mediatorMock.Object, _certRepoMock.Object, _studentRepoMock.Object, _uowMock.Object);
        }

        [Fact]
        public async Task BulkIssueCertificates_ReturnsOk()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<BulkIssueCertificatesCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new { totalCount = 1 });

            var result = await _controller.BulkIssueCertificates(new BulkIssueCertificatesCommand());
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task VerifyCertificate_ReturnsOk()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<VerifyCertificateQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new { status = "VALID" });

            var result = await _controller.VerifyCertificate(Guid.NewGuid().ToString());
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }
    }
}
