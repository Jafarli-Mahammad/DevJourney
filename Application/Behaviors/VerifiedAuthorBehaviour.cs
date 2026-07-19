using Application.Exceptions;
using Application.Repositories;
using Application.Services;
using MediatR;

namespace Application.Behaviors
{
    public class VerifiedAuthorBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, IRequireVerifiedAuthor
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ICompanyProfileRepository _companyProfileRepository;

        public VerifiedAuthorBehaviour(ICurrentUserService currentUserService, ICompanyProfileRepository companyProfileRepository)
        {
            _currentUserService = currentUserService;
            _companyProfileRepository = companyProfileRepository;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            
            var companyProfile = await _companyProfileRepository.GetAsync(c => c.ApplicationUserId == userId, cancellationToken: cancellationToken);
            if (companyProfile == null || !companyProfile.IsVerified)
            {
                throw new UnauthorizedException("Only verified companies can perform this action.");
            }

            return await next();
        }
    }
}
