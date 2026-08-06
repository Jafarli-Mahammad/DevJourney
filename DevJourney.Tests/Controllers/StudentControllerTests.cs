using Application.Modules.Student.Queries.GetAllStudentProfiles;
using Application.Modules.Student.Queries.GetStudentProfile;
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
    public class StudentControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly StudentController _controller;

        public StudentControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new StudentController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetStudentProfile_ReturnsOk_WhenProfileExists()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var expectedProfile = new StudentProfileDto { Id = studentId, FirstName = "John", LastName = "Doe" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetStudentProfileQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedProfile);

            // Act
            var result = await _controller.GetStudentProfile(studentId, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualProfile = Assert.IsType<StudentProfileDto>(okResult.Value);
            Assert.Equal(studentId, actualProfile.Id);
        }

        [Fact]
        public async Task GetStudentProfile_ReturnsNotFound_WhenProfileDoesNotExist()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetStudentProfileQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync((StudentProfileDto)null!);

            // Act
            var result = await _controller.GetStudentProfile(studentId, CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAllStudentProfiles_ReturnsOk_WithProfiles()
        {
            // Arrange
            var expectedProfiles = new List<StudentProfileListDto>
            {
                new StudentProfileListDto { Id = Guid.NewGuid(), Email = "student1@example.com" },
                new StudentProfileListDto { Id = Guid.NewGuid(), Email = "student2@example.com" }
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllStudentProfilesQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedProfiles);

            // Act
            var result = await _controller.GetAllStudentProfiles(CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualProfiles = Assert.IsType<List<StudentProfileListDto>>(okResult.Value);
            Assert.Equal(2, actualProfiles.Count);
        }
    }
}
