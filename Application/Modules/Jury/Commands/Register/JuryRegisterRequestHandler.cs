using Application.Repositories;
using Application.Services;
using Domain.Models.Entities.Jury;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Jury.Commands.Register
{
    public class JuryRegisterRequestHandler : IRequestHandler<JuryRegisterRequest, Guid>
    {
        private readonly IAuthService _authService;
        private readonly IJuryProfileRepository _juryProfileRepository;
        private readonly IUnitOfWork _unitOfWork;

        public JuryRegisterRequestHandler(
            IAuthService authService,
            IJuryProfileRepository juryProfileRepository,
            IUnitOfWork unitOfWork)
        {
            _authService = authService;
            _juryProfileRepository = juryProfileRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(JuryRegisterRequest request, CancellationToken cancellationToken)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var userId = await _authService.RegisterAsync(request.JuryCode, request.Email, request.Password);

                var juryProfile = new JuryProfile(
                    userId,
                    request.JuryCode,
                    request.FullName,
                    request.Email
                )
                {
                    Specialization = request.Specialization,
                    CompetitionId = request.CompetitionId
                };

                await _juryProfileRepository.AddAsync(juryProfile, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return juryProfile.Id;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
