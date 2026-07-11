namespace Application.Services
{
    public interface IAuthService
    {
        Task<Guid> RegisterAsync(string userName, string email, string password);
        Task<bool> CheckPasswordAsync(string email, string password);
    }
}
