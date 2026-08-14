using Application.Modules.Competitions.Commands.CreateCompetition;
using Application.Repositories;
using Application.Repositories.Competitions;
using Domain.Models.Entities.Competition;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Competitions.Commands.CreateCompetition
{
    public class CreateCompetitionHandler : IRequestHandler<CreateCompetitionCommand, Guid>
    {
        private readonly ICompetitionRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCompetitionHandler(ICompetitionRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateCompetitionCommand request, CancellationToken cancellationToken)
        {
            var competition = new Competition
            {
                PartnerId = request.PartnerId,
                Title = request.Dto.Title,
                ShortSummary = request.Dto.ShortSummary,
                Description = request.Dto.Description,
                ParticipationFormat = request.Dto.ParticipationFormat,
                MaxTeamSize = request.Dto.MaxTeamSize,
                StartDate = request.Dto.StartDate,
                EndDate = request.Dto.EndDate,
                RegistrationDeadline = request.Dto.RegistrationDeadline,
                Location = request.Dto.Location,
                LocationMapLink = request.Dto.LocationMapLink,
                Tags = request.Dto.Tags,
                EvaluationCriteria = request.Dto.EvaluationCriteria,
                CoverImageUrl = request.Dto.CoverImageUrl,
                ContactEmail = request.Dto.ContactEmail,
                ContactPhone = request.Dto.ContactPhone,
                ContactSocialLink = request.Dto.ContactSocialLink,
                SubmissionDeadline = request.Dto.SubmissionDeadline,
                GitHubRepositoryRequirement = request.Dto.GitHubRepositoryRequirement,
                LiveDeploymentRequirement = request.Dto.LiveDeploymentRequirement,
                PitchDeckFormat = request.Dto.PitchDeckFormat,
                IsPublished = false,
                Stages = request.Dto.Stages?.Select(s => new CompetitionStage
                {
                    DayNumber = s.DayNumber,
                    Title = s.Title,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime
                }).ToList() ?? new System.Collections.Generic.List<CompetitionStage>()
            };

            await _repository.AddAsync(competition, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return competition.Id;
        }
    }
}
