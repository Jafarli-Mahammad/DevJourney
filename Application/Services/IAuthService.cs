namespace Application.Services
{
    public interface IAuthService
    {
        Task<Guid> RegisterAsync(string userName, string email, string password);
        Task<bool> CheckPasswordAsync(string email, string password);
        Task<bool> CheckPasswordByUserNameAsync(string userName, string password);
        Task<(Guid UserId, string UserName, string Email)?> GetUserInfoByEmailAsync(string email);
        Task<(Guid UserId, string UserName, string Email)?> GetUserInfoByNameAsync(string userName);
        Task<(bool Succeeded, string[] Errors)> ResetPasswordAsync(string email, string token, string newPassword);
    }
}
