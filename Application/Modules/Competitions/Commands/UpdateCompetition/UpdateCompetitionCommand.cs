using Application.Exceptions;
using Application.Modules.Competitions.Dtos;
using Application.Repositories;
using Application.Repositories.Competitions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Competitions.Commands.UpdateCompetition
{
    public class UpdateCompetitionCommand : IRequest<Guid>
    {
        public Guid CompetitionId { get; set; }
        public CreateCompetitionDto Dto { get; set; } = null!;
    }

    public class UpdateCompetitionCommandHandler : IRequestHandler<UpdateCompetitionCommand, Guid>
    {
        private readonly ICompetitionRepository _competitionRepo;

        public UpdateCompetitionCommandHandler(ICompetitionRepository competitionRepo)
        {
            _competitionRepo = competitionRepo;
        }

        public async Task<Guid> Handle(UpdateCompetitionCommand request, CancellationToken cancellationToken)
        {
            var comp = await _competitionRepo.GetAsync(c => c.Id == request.CompetitionId, null, cancellationToken);
            if (comp == null) throw new NotFoundException("Competition", request.CompetitionId);

            comp.Title = request.Dto.Title;
            comp.ShortSummary = request.Dto.ShortSummary;
            comp.Description = request.Dto.Description;
            comp.StartDate = request.Dto.StartDate;
            comp.EndDate = request.Dto.EndDate;
            comp.RegistrationDeadline = request.Dto.RegistrationDeadline;
            comp.Location = request.Dto.Location ?? "";
            comp.LocationMapLink = request.Dto.LocationMapLink ?? "";
            comp.Tags = request.Dto.Tags != null ? string.Join(",", request.Dto.Tags) : "";
            comp.CoverImageUrl = request.Dto.CoverImageUrl ?? "";
            comp.ContactEmail = request.Dto.ContactEmail ?? "";
            comp.ContactPhone = request.Dto.ContactPhone ?? "";
            comp.ContactSocialLink = request.Dto.ContactSocialLink ?? "";
            comp.SubmissionDeadline = request.Dto.SubmissionDeadline;
            comp.ParticipationFormat = request.Dto.ParticipationFormat;
            comp.MaxTeamSize = request.Dto.MaxTeamSize;
            comp.GitHubRepositoryRequirement = request.Dto.GitHubRepositoryRequirement;
            comp.LiveDeploymentRequirement = request.Dto.LiveDeploymentRequirement;
            comp.PitchDeckFormat = request.Dto.PitchDeckFormat;
            
            await _competitionRepo.EditAsync(comp);

            return comp.Id;
        }
    }
}
