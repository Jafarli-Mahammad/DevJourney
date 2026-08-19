using Application.Exceptions;
using Application.Modules.Student.Commands.UpdateCabinetProfile;
using Application.Modules.Student.Queries.GetStudentProfile;
using Application.Repositories;
using Application.Services;
using Domain.Models.Entities.Student;
using Moq;

namespace DevJourney.Tests.Handlers
{
    public class UpdateStudentProfileCommandHandlerTests
    {
        private readonly Mock<IStudentProfileRepository> _repositoryMock;
        private readonly Mock<IUniversityProfileRepository> _universityRepositoryMock;
        private readonly Mock<IProfessionRepository> _professionRepositoryMock;
        private readonly Mock<IMainRoleRepository> _mainRoleRepositoryMock;
        private readonly Mock<ISkillRepository> _skillRepositoryMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UpdateStudentProfileCommandHandler _handler;

        public UpdateStudentProfileCommandHandlerTests()
        {
            _repositoryMock = new Mock<IStudentProfileRepository>();
            _universityRepositoryMock = new Mock<IUniversityProfileRepository>();
            _professionRepositoryMock = new Mock<IProfessionRepository>();
            _mainRoleRepositoryMock = new Mock<IMainRoleRepository>();
            _skillRepositoryMock = new Mock<ISkillRepository>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _handler = new UpdateStudentProfileCommandHandler(
                _repositoryMock.Object,
                _universityRepositoryMock.Object,
                _professionRepositoryMock.Object,
                _mainRoleRepositoryMock.Object,
                _skillRepositoryMock.Object,
                _currentUserServiceMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_UpdatesProfileDetailsAndReturnsDto_WhenProfileExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingProfile = new StudentProfile(userId, "John", "Doe");

            var command = new UpdateStudentProfileCommand
            {
                PhoneNumber = "+994509999999",
                Course = "3-ci kurs",
                Bio = "Software developer student"
            };

            _currentUserServiceMock.Setup(c => c.IsAuthenticated).Returns(true);
            _currentUserServiceMock.Setup(c => c.UserId).Returns(userId);

            _repositoryMock.Setup(r => r.GetFullProfileByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(existingProfile);

            _repositoryMock.Setup(r => r.GetWithEmailByIdAsync(existingProfile.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((existingProfile, "john@example.com"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);
            Assert.Equal("+994509999999", result.PhoneNumber);
            Assert.Equal("3-ci kurs", result.Course);
            Assert.Equal("Software developer student", result.Bio);

            _repositoryMock.Verify(r => r.EditAsync(existingProfile), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ThrowsUnauthorizedException_WhenNotAuthenticated()
        {
            // Arrange
            var command = new UpdateStudentProfileCommand();
            _currentUserServiceMock.Setup(c => c.IsAuthenticated).Returns(false);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
