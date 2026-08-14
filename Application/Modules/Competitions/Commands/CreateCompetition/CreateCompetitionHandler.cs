using Application.Modules.Competitions.Commands.CreateCompetition;
using Application.Repositories;
using Application.Repositories.Competitions;
using Domain.Models.Entities.Competition;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Competitions.Commands.CreateCompetition
{
    public class CreateCompetitionHandler : IRequestHandler<CreateCompetitionCommand, Guid>
    {
        private readonly ICompetitionRepository _repository;
        private readonly IPartnerProfileRepository _partnerProfileRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCompetitionHandler(
            ICompetitionRepository repository,
            IPartnerProfileRepository partnerProfileRepository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _partnerProfileRepository = partnerProfileRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateCompetitionCommand request, CancellationToken cancellationToken)
        {
            var partnerId = request.PartnerId;
            var partners = await _partnerProfileRepository.GetAllAsync(cancellationToken: cancellationToken);
            
            var existingPartner = partners.FirstOrDefault(p => p.Id == partnerId);
            if (existingPartner == null && partners.Any())
            {
                partnerId = partners.First().Id;
            }

            var competition = new Competition
            {
                PartnerId = partnerId,
                Title = request.Dto.Title ?? string.Empty,
                ShortSummary = request.Dto.ShortSummary ?? string.Empty,
                Description = request.Dto.Description ?? string.Empty,
                ParticipationFormat = request.Dto.ParticipationFormat,
                MaxTeamSize = request.Dto.MaxTeamSize,
                StartDate = request.Dto.StartDate,
                EndDate = request.Dto.EndDate,
                RegistrationDeadline = request.Dto.RegistrationDeadline,
                Location = request.Dto.Location ?? string.Empty,
                LocationMapLink = request.Dto.LocationMapLink ?? string.Empty,
                Tags = request.Dto.Tags ?? string.Empty,
                EvaluationCriteria = request.Dto.EvaluationCriteria ?? string.Empty,
                CoverImageUrl = request.Dto.CoverImageUrl ?? string.Empty,
                ContactEmail = request.Dto.ContactEmail ?? string.Empty,
                ContactPhone = request.Dto.ContactPhone ?? string.Empty,
                ContactSocialLink = request.Dto.ContactSocialLink ?? string.Empty,
                SubmissionDeadline = request.Dto.SubmissionDeadline,
                GitHubRepositoryRequirement = request.Dto.GitHubRepositoryRequirement,
                LiveDeploymentRequirement = request.Dto.LiveDeploymentRequirement,
                PitchDeckFormat = request.Dto.PitchDeckFormat,
                IsPublished = false,
                Stages = request.Dto.Stages?.Where(s => s != null).Select(s => new CompetitionStage
                {
                    DayNumber = s.DayNumber,
                    Title = s.Title ?? string.Empty,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime
                }).ToList() ?? new List<CompetitionStage>()
            };

            await _repository.AddAsync(competition, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return competition.Id;
        }
    }
}
