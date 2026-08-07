using Application.Seeder;
using DataAccessLayer.DataContexts;
using Domain.Models.Entities.Student;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Seeders
{
    public class ProfessionSeeder : IDataSeeder
    {
        private readonly DataContext dataContext;

        public ProfessionSeeder(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task SeedAsync()
        {
            var existingNames = await dataContext.Professions
                .Select(p => p.Name)
                .ToListAsync();

            var professions = new List<Profession>
            {
                new() { Id = Guid.NewGuid(), Name = "Kompüter Elmləri" },
                new() { Id = Guid.NewGuid(), Name = "Kompüter Mühəndisliyi" },
                new() { Id = Guid.NewGuid(), Name = "İnformasiya Texnologiyaları" },
                new() { Id = Guid.NewGuid(), Name = "İnformasiya Təhlükəsizliyi" },
                new() { Id = Guid.NewGuid(), Name = "Proqram Təminatı Mühəndisliyi" },
                new() { Id = Guid.NewGuid(), Name = "Süni İntellekt və Məlumat Analitikası" },
                new() { Id = Guid.NewGuid(), Name = "Sistem Mühəndisliyi" },
                new() { Id = Guid.NewGuid(), Name = "Rəqəmsal Dizayn və Media" },
                new() { Id = Guid.NewGuid(), Name = "Biznes İdarəçiliyi" },
                new() { Id = Guid.NewGuid(), Name = "İqtisadiyyat və Maliyyə" }
            };

            var newProfessions = professions
                .Where(p => !existingNames.Contains(p.Name))
                .ToList();

            if (!newProfessions.Any()) return;

            await dataContext.Professions.AddRangeAsync(newProfessions);
            await dataContext.SaveChangesAsync();
        }
    }
}
