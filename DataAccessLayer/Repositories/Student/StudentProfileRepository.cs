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

            return list.ConvertAll(x => (x.Profile, x.Email));
        }

        public async Task<(StudentProfile Profile, string? Email)?> GetWithEmailByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var profile = await GetFullProfileByIdAsync(id, cancellationToken);
            if (profile == null) return null;

            var user = await DataContext.Users
                .FirstOrDefaultAsync(u => u.Id == profile.ApplicationUserId, cancellationToken);

            return (profile, user?.Email);
        }

        public async Task<StudentProfile?> GetFullProfileByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await DataContext.StudentProfiles
                .Include(sp => sp.University)
                .Include(sp => sp.Profession)
                .Include(sp => sp.MainRole)
                .Include(sp => sp.StudentSkills)
                    .ThenInclude(ss => ss.Skill)
                .Include(sp => sp.StudentLanguages)
                    .ThenInclude(sl => sl.Language)
                .AsSplitQuery()
                .FirstOrDefaultAsync(sp => sp.Id == id, cancellationToken);
        }

        public async Task<StudentProfile?> GetFullProfileByUserIdAsync(Guid applicationUserId, CancellationToken cancellationToken = default)
        {
            return await DataContext.StudentProfiles
                .Include(sp => sp.University)
                .Include(sp => sp.Profession)
                .Include(sp => sp.MainRole)
                .Include(sp => sp.StudentSkills)
                    .ThenInclude(ss => ss.Skill)
                .Include(sp => sp.StudentLanguages)
                    .ThenInclude(sl => sl.Language)
                .AsSplitQuery()
                .FirstOrDefaultAsync(sp => sp.ApplicationUserId == applicationUserId, cancellationToken);
        }
    }
}