using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Modules.PartnerProfile.Commands.UpdatePartnerProfile;
using Application.Modules.PartnerProfile.Queries.GetPartnerProfile;
using Devjourney.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DevJourney.Tests.Controllers
{
    public class PartnerProfileControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly PartnerProfileController _controller;

        public PartnerProfileControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new PartnerProfileController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetProfile_ReturnsOk()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPartnerProfileQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new { PartnerName = "Test Partner" });

            var result = await _controller.GetProfile(CancellationToken.None);
            
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task UpdateProfile_ReturnsOk()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdatePartnerProfileCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new { Id = Guid.NewGuid() });

            var command = new UpdatePartnerProfileCommand { PartnerName = "New Name" };
            var result = await _controller.UpdateProfile(command, CancellationToken.None);
            
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }
    }
}
