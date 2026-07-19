using Application.Seeder;
using DataAccessLayer.DataContexts;
using Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Seeders
{
    public class RoleSeeder : IDataSeeder
    {
        private readonly DataContext dataContext;

        public RoleSeeder(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task SeedAsync()
        {
            // Idempotent: only seed when the lookup table is empty
            if (await dataContext.LookupRoles.AnyAsync())
                return;

            var roles = new List<Role>
            {
                new() { Id = Guid.NewGuid(), Name = "Programmer" },
                new() { Id = Guid.NewGuid(), Name = "Designer" },
                new() { Id = Guid.NewGuid(), Name = "QA" },
                new() { Id = Guid.NewGuid(), Name = "DevOps" },
                new() { Id = Guid.NewGuid(), Name = "PM" },
                new() { Id = Guid.NewGuid(), Name = "Data Analyst" },
                new() { Id = Guid.NewGuid(), Name = "Robotics Developer" },
            };

            await dataContext.LookupRoles.AddRangeAsync(roles);
            await dataContext.SaveChangesAsync();
        }
    }
}
