using Application.Repositories;
using DataAccessLayer.DataContexts;
using Domain.Models.Entities.Student;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class StudentProfileRepository : AsyncRepository<StudentProfile>, IStudentProfileRepository
    {
        public StudentProfileRepository(DataContext dataContext)
            : base(dataContext)
        {
        }

        public async Task<StudentProfile?> GetByIdAsync(Guid id)
        {
            return await GetAsync(profile => profile.Id == id);
        }

        public async Task<StudentProfile?> GetByUserIdAsync(Guid applicationUserId)
        {
            return await GetAsync(profile => profile.ApplicationUserId == applicationUserId);
        }

        public async Task<bool> ExistsAsync(Guid applicationUserId)
        {
            return await DataContext.StudentProfiles
                .AnyAsync(profile => profile.ApplicationUserId == applicationUserId);
        }
    }
}