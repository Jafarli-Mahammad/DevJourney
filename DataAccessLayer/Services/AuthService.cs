using Application.Services;
using DataAccessLayer.IdentityEntities;
using Microsoft.AspNetCore.Identity;

namespace DataAccessLayer.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> userManager;

        public AuthService(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<bool> CheckPasswordAsync(string email, string password)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return false;
            return await userManager.CheckPasswordAsync(user, password);
        }

        public async Task<Guid> RegisterAsync(string firstName, string lastName, string userName, string email, string password)
        {
            var user = new ApplicationUser
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
                Email = email,
            };

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            return user.Id;
        }
    }
}
