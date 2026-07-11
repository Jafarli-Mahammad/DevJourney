using Application.Repositories;
using Application.Services;
using Domain.Models.Entities.Company;
using MediatR;

namespace Application.Modules.Company.Commands.Register
{
    public class CompanyRegisterRequestHandler : IRequestHandler<CompanyRegisterRequest, Guid>
    {
        private readonly ICompanyProfileRepository companyProfileRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IAuthService authService;

        public CompanyRegisterRequestHandler(ICompanyProfileRepository companyProfileRepository, IUnitOfWork unitOfWork, IAuthService authService)
        {
            this.companyProfileRepository = companyProfileRepository;
            this.unitOfWork = unitOfWork;
            this.authService = authService;
        }

        public async Task<Guid> Handle(CompanyRegisterRequest request, CancellationToken cancellationToken)
        {
            using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var userId = await authService.RegisterAsync(
                    request.UserName, request.Email, request.Password);

                var profile = new CompanyProfile(userId, request.CompanyName, request.CompanySize);
                profile.UpdateProfile(
                    request.CompanySector, request.WebsiteUrl, request.LinkedInUrl,
                    request.Location, request.RepresentativeName, request.RepresentativeEmail);

                await companyProfileRepository.AddAsync(profile, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
                return userId;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}