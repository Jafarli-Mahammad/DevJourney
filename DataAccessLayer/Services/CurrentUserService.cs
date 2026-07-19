using Application.Exceptions;
using Application.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DataAccessLayer.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
            => _httpContextAccessor = httpContextAccessor;

        public Guid UserId
        {
            get
            {
                var idClaim = _httpContextAccessor.HttpContext?.User?
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value;

                return Guid.TryParse(idClaim, out var id)
                    ? id
                    : throw new UnauthorizedException("No authenticated user found.");
            }
        }

        public string? Email =>
            _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}