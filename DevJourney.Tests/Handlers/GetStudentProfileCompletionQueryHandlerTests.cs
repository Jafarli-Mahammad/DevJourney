using Application.Modules.Student.Queries.GetStudentProfileCompletion;
using Application.Repositories;
using Domain.Models.Entities.Student;
using Moq;

namespace DevJourney.Tests.Handlers
{
    public class GetStudentProfileCompletionQueryHandlerTests
    {
        private readonly Mock<IStudentProfileRepository> _repositoryMock;
        private readonly GetStudentProfileCompletionQueryHandler _handler;

        public GetStudentProfileCompletionQueryHandlerTests()
        {
            _repositoryMock = new Mock<IStudentProfileRepository>();
            _handler = new GetStudentProfileCompletionQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsCompletionDto_WhenProfileExists()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var profile = new StudentProfile(userId, "Jane", "Doe");

            _repositoryMock.Setup(r => r.GetFullProfileByIdAsync(studentId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(profile);

            var query = new GetStudentProfileCompletionQuery(studentId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(profile.Id, result.StudentProfileId);
            Assert.True(result.CompletionPercentage >= 0 && result.CompletionPercentage <= 100);
        }

        [Fact]
        public async Task Handle_ReturnsNull_WhenProfileDoesNotExist()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetFullProfileByIdAsync(studentId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((StudentProfile?)null);

            var query = new GetStudentProfileCompletionQuery(studentId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }
    }
}
