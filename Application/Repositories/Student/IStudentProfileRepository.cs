using Domain.Models.Entities.Student;

namespace Application.Repositories
{
    public interface IStudentProfileRepository : IAsyncRepository<StudentProfile>
    {
        Task<StudentProfile?> GetByIdAsync(Guid id);
        Task<StudentProfile?> GetByUserIdAsync(Guid applicationUserId);
        Task<bool> ExistsAsync(Guid applicationUserId);
        Task<List<(StudentProfile Profile, string? Email)>> GetAllWithEmailAsync(CancellationToken cancellationToken = default);
        Task<(StudentProfile Profile, string? Email)?> GetWithEmailByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<StudentProfile?> GetFullProfileByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<StudentProfile?> GetFullProfileByUserIdAsync(Guid applicationUserId, CancellationToken cancellationToken = default);
    }
}