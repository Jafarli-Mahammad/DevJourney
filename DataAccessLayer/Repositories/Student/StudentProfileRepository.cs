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

        public async Task<List<(StudentProfile Profile, string? Email)>> GetAllWithEmailAsync(CancellationToken cancellationToken = default)
        {
            var list = await (from sp in DataContext.StudentProfiles
                              join u in DataContext.Users on sp.ApplicationUserId equals u.Id into usersGroup
                              from u in usersGroup.DefaultIfEmpty()
                              select new
                              {
                                  Profile = sp,
                                  Email = u != null ? u.Email : null
                              }).ToListAsync(cancellationToken);

            return list.Select(x => (x.Profile, x.Email)).ToList();
        }

        public async Task<(StudentProfile Profile, string? Email)?> GetWithEmailByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var item = await (from sp in DataContext.StudentProfiles
                              where sp.Id == id
                              join u in DataContext.Users on sp.ApplicationUserId equals u.Id into usersGroup
                              from u in usersGroup.DefaultIfEmpty()
                              select new
                              {
                                  Profile = sp,
                                  Email = u != null ? u.Email : null
                              }).FirstOrDefaultAsync(cancellationToken);

            if (item == null) return null;
            return (item.Profile, item.Email);
        }
    }
}