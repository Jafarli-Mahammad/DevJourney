using Application.Seeder;
using DataAccessLayer.DataContexts;
using DataAccessLayer.IdentityEntities;
using Domain.Models.Entities.University;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Seeders
{
    public class UniversitySeeder : IDataSeeder
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public UniversitySeeder(DataContext dataContext, UserManager<ApplicationUser> userManager)
        {
            _dataContext = dataContext;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            var existingNames = await _dataContext.UniversityProfiles
                .Select(u => u.UniversityName)
                .ToListAsync();

            var universitiesToSeed = new List<string>
            {
                "Baku State University (BDU)",
                "Azerbaijan State Oil and Industry University (ADNSU)",
                "Azerbaijan Technical University (AzTU)",
                "Baku Engineering University (BMU)",
                "ADA University",
                "Azerbaijan State University of Economics (UNEC)",
                "Khazar University",
                "Baku Higher Oil School (BANM)",
                "National Aviation Academy (MAA)"
            };

            foreach (var name in universitiesToSeed)
            {
                if (!existingNames.Contains(name))
                {
                    var cleanName = new string(name.Where(char.IsLetterOrDigit).ToArray()).ToLower();
                    var dummyEmail = $"{cleanName}@dummyuni.edu";
                    var user = new ApplicationUser
                    {
                        Id = Guid.NewGuid(),
                        UserName = dummyEmail,
                        Email = dummyEmail,
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(user, "Uni123456!");
                    if (result.Succeeded)
                    {
                        var profile = new UniversityProfile(user.Id, name);
                        await _dataContext.UniversityProfiles.AddAsync(profile);
                    }
                }
            }

            await _dataContext.SaveChangesAsync();
        }
    }
}
