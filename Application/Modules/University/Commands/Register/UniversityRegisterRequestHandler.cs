using Application.Repositories;
using Application.Services;
using Domain.Models.Entities.Company;
using Domain.Models.Entities.University;
using MediatR;

namespace Application.Modules.University.Commands.Register
{
    public class UniversityRegisterRequestHandler : IRequestHandler<UniversityRegisterRequest, Guid>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUniversityProfileRepository universityProfileRepository;
        private readonly IAuthService authService;

        public UniversityRegisterRequestHandler(IUnitOfWork unitOfWork, IUniversityProfileRepository universityProfileRepository, IAuthService authService)
        {
            this.unitOfWork = unitOfWork;
            this.universityProfileRepository = universityProfileRepository;
            this.authService = authService;
        }

        public async Task<Guid> Handle(UniversityRegisterRequest request, CancellationToken cancellationToken)
        {
            using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var userId = await authService.RegisterAsync(
                    request.UserName, request.Email, request.Password);

                var profile = new UniversityProfile(userId, request.UniversityName);

                profile.UpdateProfile(
                    request.WebsiteUrl,
                    request.Location, request.RepresentativeName, request.RepresentativeEmail);

                await universityProfileRepository.AddAsync(profile, cancellationToken);
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
