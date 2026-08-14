using Application.Seeder;
using DataAccessLayer.DataContexts;
using DataAccessLayer.IdentityEntities;
using Domain.Models.Entities.Partner;
using Domain.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DataAccessLayer.Seeders
{
    public class PartnerSeeder : IDataSeeder
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public PartnerSeeder(DataContext dataContext, UserManager<ApplicationUser> userManager)
        {
            _dataContext = dataContext;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            if (await _dataContext.PartnerProfiles.AnyAsync())
                return;

            var email = "partner@devjourney.az";
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, "Partner123456!");
                if (!result.Succeeded)
                    return;
            }

            var partner = new PartnerProfile
            {
                Id = Guid.NewGuid(),
                ApplicationUserId = user.Id,
                PartnerName = "DevJourney Official",
                PartnerType = PartnerType.Community,
                IsVerified = true,
                CreatedAt = DateTime.UtcNow
            };

            await _dataContext.PartnerProfiles.AddAsync(partner);
            await _dataContext.SaveChangesAsync();
        }
    }
}
