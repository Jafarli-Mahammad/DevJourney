using Domain.Models.Entities.Student;

namespace Application.Repositories
{
    public interface IStudentProfileRepository : IAsyncRepository<StudentProfile>
    {
        Task<StudentProfile?> GetByIdAsync(Guid id);
        Task<StudentProfile?> GetByUserIdAsync(Guid applicationUserId);
        Task<bool> ExistsAsync(Guid applicationUserId);
    }
}