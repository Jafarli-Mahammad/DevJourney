using System.Security.Claims;

namespace Application.Services
{
    public interface IJwtService
    {
        string GenerateAccessToken(Guid userId, string userName, string email, IEnumerable<Claim>? extraClaims = null);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
