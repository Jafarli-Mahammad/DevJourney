using Application.Seeder;
using DataAccessLayer.DataContexts;
using Domain.Models.Entities.Student;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Seeders
{
    public class LanguageSeeder : IDataSeeder
    {
        private readonly DataContext dataContext;

        public LanguageSeeder(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task SeedAsync()
        {
            var existingNames = await dataContext.Skills
                .Select(s => s.Name)
                .ToListAsync();

            var languages = new List<Language>
            {
                new() { Id = Guid.NewGuid(), Name = "Azerbaijani" },
                new() { Id = Guid.NewGuid(), Name = "Turkish" },
                new() { Id = Guid.NewGuid(), Name = "Russian" },

                // Qlobal və populyar iş dünyası dilləri
                new() { Id = Guid.NewGuid(), Name = "English" },
                new() { Id = Guid.NewGuid(), Name = "German" },
                new() { Id = Guid.NewGuid(), Name = "French" },
                new() { Id = Guid.NewGuid(), Name = "Spanish" },
                new() { Id = Guid.NewGuid(), Name = "Italian" },
                new() { Id = Guid.NewGuid(), Name = "Chinese" },
                new() { Id = Guid.NewGuid(), Name = "Arabic" },
                new() { Id = Guid.NewGuid(), Name = "Japanese" },
                new() { Id = Guid.NewGuid(), Name = "Korean" },

                // Digər Avropa və Şərq dilləri
                new() { Id = Guid.NewGuid(), Name = "Polish" },
                new() { Id = Guid.NewGuid(), Name = "Ukrainian" },
                new() { Id = Guid.NewGuid(), Name = "Persian" },
                new() { Id = Guid.NewGuid(), Name = "Georgian" }
            };

            var newLanguages = languages
                .Where(s => !existingNames.Contains(s.Name))
                .ToList();

            if (!newLanguages.Any()) return;

            await dataContext.Languages.AddRangeAsync(newLanguages);
            await dataContext.SaveChangesAsync();
        }
    }
}
