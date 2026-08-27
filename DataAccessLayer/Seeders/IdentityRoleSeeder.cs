using Application.Seeder;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Seeders
{
    public class IdentityRoleSeeder : IDataSeeder
    {
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public IdentityRoleSeeder(RoleManager<IdentityRole<Guid>> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            var roles = new List<string>
            {
                "JURY",
                "SUPPORTER",
                "COMPANY_ADMIN",
                "Student",
                "Admin",
                "PARTNER",
                "UNIVERSITY_ADMIN",
                "VOLUNTEER"
            };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole<Guid>
                    {
                        Name = role,
                        NormalizedName = role.ToUpperInvariant()
                    });
                }
            }
        }
    }
}
