using Application.Exceptions;
using Application.Repositories;
using Application.Services;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Behaviors
{
    public class VerifiedAuthorBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, IRequireVerifiedAuthor
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ICompanyProfileRepository _companyProfileRepository;
        private readonly IMemoryCache _cache;

        public VerifiedAuthorBehaviour(
            ICurrentUserService currentUserService,
            ICompanyProfileRepository companyProfileRepository,
            IMemoryCache cache)
        {
            _currentUserService = currentUserService;
            _companyProfileRepository = companyProfileRepository;
            _cache = cache;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var cacheKey = $"verified-author:{userId}";

            if (!_cache.TryGetValue(cacheKey, out bool isVerified))
            {
                var companyProfile = await _companyProfileRepository.GetAsync(c => c.ApplicationUserId == userId, cancellationToken: cancellationToken);
                isVerified = companyProfile != null && companyProfile.IsVerified;
                _cache.Set(cacheKey, isVerified, TimeSpan.FromMinutes(5));
            }

            if (!isVerified)
            {
                throw new UnauthorizedException("Only verified companies can perform this action.");
            }

            return await next();
        }
    }
}
