using Application.Modules.Student.Commands.UpdateCabinetProfile;
using Application.Modules.Student.Queries.GetAllStudentProfiles;
using Application.Modules.Student.Queries.GetStudentProfile;
using Application.Modules.Student.Queries.GetStudentProfileCompletion;
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
                new StudentProfileListDto { Id = Guid.NewGuid(), FirstName = "student1" },
                new StudentProfileListDto { Id = Guid.NewGuid(), FirstName = "student2" }
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

        [Fact]
        public async Task UpdateStudentProfile_ReturnsOk_WithUpdatedProfile()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var command = new UpdateStudentProfileCommand
            {
                StudentProfileId = studentId,
                PhoneNumber = "+994501234567",
                Bio = "Updated bio"
            };
            var expectedProfile = new StudentProfileDto
            {
                Id = studentId,
                PhoneNumber = "+994501234567",
                Bio = "Updated bio",
                CompletionPercentage = 20
            };

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedProfile);

            // Act
            var result = await _controller.UpdateStudentProfile(command, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualProfile = Assert.IsType<StudentProfileDto>(okResult.Value);
            Assert.Equal(studentId, actualProfile.Id);
            Assert.Equal("+994501234567", actualProfile.PhoneNumber);
        }

        [Fact]
        public async Task GetProfileCompletion_ReturnsOk_WhenProfileExists()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var expectedCompletion = new ProfileCompletionDto
            {
                StudentProfileId = studentId,
                CompletionPercentage = 75
            };

            _mediatorMock.Setup(m => m.Send(It.Is<GetStudentProfileCompletionQuery>(q => q.StudentId == studentId), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedCompletion);

            // Act
            var result = await _controller.GetProfileCompletion(studentId, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualCompletion = Assert.IsType<ProfileCompletionDto>(okResult.Value);
            Assert.Equal(studentId, actualCompletion.StudentProfileId);
            Assert.Equal(75, actualCompletion.CompletionPercentage);
        }

        [Fact]
        public async Task GetProfileCompletion_ReturnsNotFound_WhenProfileDoesNotExist()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetStudentProfileCompletionQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync((ProfileCompletionDto)null!);

            // Act
            var result = await _controller.GetProfileCompletion(studentId, CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
