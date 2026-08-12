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

        public async Task<bool> CheckPasswordByUserNameAsync(string userName, string password)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user == null) return false;
            return await userManager.CheckPasswordAsync(user, password);
        }

        public async Task<(Guid UserId, string UserName, string Email)?> GetUserInfoByEmailAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return null;
            return (user.Id, user.UserName ?? string.Empty, user.Email ?? string.Empty);
        }

        public async Task<(Guid UserId, string UserName, string Email)?> GetUserInfoByNameAsync(string userName)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user == null) return null;
            return (user.Id, user.UserName ?? string.Empty, user.Email ?? string.Empty);
        }

        public async Task<Guid> RegisterAsync(string userName, string email, string password)
        {
            var user = new ApplicationUser
            {
                UserName = userName,
                Email = email,
            };

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                throw new Application.Exceptions.BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

            return user.Id;
        }
    }
}
