using Application.Modules.Jury.Queries.GetAllJuryProfiles;
using Application.Modules.Jury.Queries.GetJuryProfile;
using Devjourney.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DevJourney.Tests.Controllers
{
    public class JuryControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly JuryController _controller;

        public JuryControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new JuryController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetJuryProfile_ReturnsOk_WhenProfileExists()
        {
            // Arrange
            var juryId = Guid.NewGuid();
            var expectedProfile = new JuryProfileDto { Id = juryId, JuryCode = "JURY-123" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetJuryProfileQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedProfile);

            // Act
            var result = await _controller.GetJuryProfile(juryId, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualProfile = Assert.IsType<JuryProfileDto>(okResult.Value);
            Assert.Equal(juryId, actualProfile.Id);
        }

        [Fact]
        public async Task GetJuryProfile_ReturnsNotFound_WhenProfileDoesNotExist()
        {
            // Arrange
            var juryId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetJuryProfileQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync((JuryProfileDto)null!);

            // Act
            var result = await _controller.GetJuryProfile(juryId, CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAllJuryProfiles_ReturnsOk_WithProfiles()
        {
            // Arrange
            var expectedProfiles = new List<JuryProfileDto>
            {
                new JuryProfileDto { Id = Guid.NewGuid(), JuryCode = "JURY-1" },
                new JuryProfileDto { Id = Guid.NewGuid(), JuryCode = "JURY-2" }
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllJuryProfilesQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedProfiles);

            // Act
            var result = await _controller.GetAllJuryProfiles(CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualProfiles = Assert.IsType<List<JuryProfileDto>>(okResult.Value);
            Assert.Equal(2, actualProfiles.Count);
        }
    }
}
