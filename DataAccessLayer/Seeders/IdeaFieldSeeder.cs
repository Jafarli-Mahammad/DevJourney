using Application.Seeder;
using DataAccessLayer.DataContexts;
using Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Seeders
{
    public class IdeaFieldSeeder : IDataSeeder
    {
        private readonly DataContext dataContext;

        public IdeaFieldSeeder(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task SeedAsync()
        {
            // Idempotent: only seed when the lookup table is empty
            if (await dataContext.LookupIdeaFields.AnyAsync()) // ⚠ verify DbSet name below
                return;

            var ideaFields = new List<IdeaField>
            {
                new() { Id = Guid.NewGuid(), Name = "FinTech" },
                new() { Id = Guid.NewGuid(), Name = "EdTech" },
                new() { Id = Guid.NewGuid(), Name = "HealthTech" },
                new() { Id = Guid.NewGuid(), Name = "AI / Machine Learning" },
                new() { Id = Guid.NewGuid(), Name = "E-Commerce" },
                new() { Id = Guid.NewGuid(), Name = "IoT" },
                new() { Id = Guid.NewGuid(), Name = "Sustainability / GreenTech" },
                new() { Id = Guid.NewGuid(), Name = "SaaS" },
                new() { Id = Guid.NewGuid(), Name = "Gaming" },
                new() { Id = Guid.NewGuid(), Name = "Social Impact" },
            };

            await dataContext.LookupIdeaFields.AddRangeAsync(ideaFields);
            await dataContext.SaveChangesAsync();
        }
    }
}