using Application.Seeder;
using DataAccessLayer.DataContexts;
using Domain.Models.Entities.Student;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Seeders
{
    public class MainRoleSeeder : IDataSeeder
    {
        private readonly DataContext dataContext;

        public MainRoleSeeder(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task SeedAsync()
        {
            var existingNames = await dataContext.MainRoles
                .Select(r => r.Name)
                .ToListAsync();

            var mainRoles = new List<MainRole>
            {
                new() { Id = Guid.NewGuid(), Name = "Frontend Developer" },
                new() { Id = Guid.NewGuid(), Name = "Backend Developer" },
                new() { Id = Guid.NewGuid(), Name = "Full Stack Developer" },
                new() { Id = Guid.NewGuid(), Name = "Mobile Developer" },
                new() { Id = Guid.NewGuid(), Name = "UI/UX Designer" },
                new() { Id = Guid.NewGuid(), Name = "DevOps Engineer" },
                new() { Id = Guid.NewGuid(), Name = "QA / Automation Tester" },
                new() { Id = Guid.NewGuid(), Name = "Data Scientist / AI Specialist" },
                new() { Id = Guid.NewGuid(), Name = "Cyber Security Specialist" },
                new() { Id = Guid.NewGuid(), Name = "Product / Project Manager" }
            };

            var newRoles = mainRoles
                .Where(r => !existingNames.Contains(r.Name))
                .ToList();

            if (!newRoles.Any()) return;

            await dataContext.MainRoles.AddRangeAsync(newRoles);
            await dataContext.SaveChangesAsync();
        }
    }
}
