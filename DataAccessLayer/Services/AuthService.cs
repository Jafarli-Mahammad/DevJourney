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

        public async Task<(Guid UserId, string UserName, string Email)?> CheckPasswordAsync(string email, string password)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return null;

            if (await userManager.IsLockedOutAsync(user))
                throw new Application.Exceptions.ForbiddenAccessException("Account is locked out.");

            var isValid = await userManager.CheckPasswordAsync(user, password);
            if (!isValid)
            {
                await userManager.AccessFailedAsync(user);
                return null;
            }

            if (user.AccessFailedCount > 0)
            {
                await userManager.ResetAccessFailedCountAsync(user);
            }
            return (user.Id, user.UserName ?? string.Empty, user.Email ?? string.Empty);
        }

        public async Task<(Guid UserId, string UserName, string Email)?> CheckPasswordByUserNameAsync(string userName, string password)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user == null) return null;

            if (await userManager.IsLockedOutAsync(user))
                throw new Application.Exceptions.ForbiddenAccessException("Account is locked out.");

            var isValid = await userManager.CheckPasswordAsync(user, password);
            if (!isValid)
            {
                await userManager.AccessFailedAsync(user);
                return null;
            }

            if (user.AccessFailedCount > 0)
            {
                await userManager.ResetAccessFailedCountAsync(user);
            }
            return (user.Id, user.UserName ?? string.Empty, user.Email ?? string.Empty);
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

        public async Task<(bool Succeeded, string[] Errors)> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // SEC: Do not reveal whether an account exists during password-reset flows
                return (true, Array.Empty<string>()); 
            }

            var result = await userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
            {
                return (false, result.Errors.Select(e => e.Description).ToArray());
            }

            return (true, Array.Empty<string>());
        }
    }
}
