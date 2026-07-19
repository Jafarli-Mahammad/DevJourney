using Application.Services;
using System.Security.Claims;

namespace DataAccessLayer.Services
{
    public class JwtService : IJwtService
    {
        public string GenerateAccessToken(Guid userId, string userName, string email, IEnumerable<Claim>? extraClaims = null)
        {
            throw new NotImplementedException();
        }

        public string GenerateRefreshToken()
        {
            throw new NotImplementedException();
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            throw new NotImplementedException();
        }
    }
}
