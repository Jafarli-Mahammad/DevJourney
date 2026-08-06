using Application.Modules.University.Queries.GetAllUniversityProfiles;
using Application.Modules.University.Queries.GetUniversityProfile;
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
    public class UniversityControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly UniversityController _controller;

        public UniversityControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new UniversityController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetUniversityProfile_ReturnsOk_WhenProfileExists()
        {
            // Arrange
            var uniId = Guid.NewGuid();
            var expectedProfile = new UniversityProfileDto { Id = uniId, UniversityName = "Tech Uni" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUniversityProfileQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedProfile);

            // Act
            var result = await _controller.GetUniversityProfile(uniId, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualProfile = Assert.IsType<UniversityProfileDto>(okResult.Value);
            Assert.Equal(uniId, actualProfile.Id);
        }

        [Fact]
        public async Task GetUniversityProfile_ReturnsNotFound_WhenProfileDoesNotExist()
        {
            // Arrange
            var uniId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUniversityProfileQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync((UniversityProfileDto)null!);

            // Act
            var result = await _controller.GetUniversityProfile(uniId, CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAllUniversityProfiles_ReturnsOk_WithProfiles()
        {
            // Arrange
            var expectedProfiles = new List<UniversityProfileDto>
            {
                new UniversityProfileDto { Id = Guid.NewGuid(), UniversityName = "Uni 1" },
                new UniversityProfileDto { Id = Guid.NewGuid(), UniversityName = "Uni 2" }
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllUniversityProfilesQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedProfiles);

            // Act
            var result = await _controller.GetAllUniversityProfiles(CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualProfiles = Assert.IsType<List<UniversityProfileDto>>(okResult.Value);
            Assert.Equal(2, actualProfiles.Count);
        }
    }
}
