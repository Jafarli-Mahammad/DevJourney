using Application.Exceptions;
using Application.Repositories;
using Application.Repositories.Competitions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Competitions.Commands.UpdateCompetitionLifecycle
{
    public class UpdateCompetitionLifecycleCommand : IRequest<object>
    {
        public Guid CompetitionId { get; set; }
        public bool? IsPublished { get; set; }
        public bool? IsRegistrationOpen { get; set; }
        public bool? IsJuryActive { get; set; }
        public bool? IsScoreboardLive { get; set; }
        public bool? IsCertificatesPublished { get; set; }
    }

    public class UpdateCompetitionLifecycleCommandHandler : IRequestHandler<UpdateCompetitionLifecycleCommand, object>
    {
        private readonly ICompetitionRepository _competitionRepo;

        public UpdateCompetitionLifecycleCommandHandler(ICompetitionRepository competitionRepo)
        {
            _competitionRepo = competitionRepo;
        }

        public async Task<object> Handle(UpdateCompetitionLifecycleCommand request, CancellationToken cancellationToken)
        {
            var comp = await _competitionRepo.GetAsync(c => c.Id == request.CompetitionId, null, cancellationToken);
            if (comp == null) throw new NotFoundException("Competition", request.CompetitionId);

            if (request.IsPublished.HasValue) comp.IsPublished = request.IsPublished.Value;
            if (request.IsRegistrationOpen.HasValue) comp.IsRegistrationOpen = request.IsRegistrationOpen.Value;
            if (request.IsJuryActive.HasValue) comp.IsJuryActive = request.IsJuryActive.Value;
            if (request.IsScoreboardLive.HasValue) comp.IsScoreboardLive = request.IsScoreboardLive.Value;
            if (request.IsCertificatesPublished.HasValue) comp.IsCertificatesPublished = request.IsCertificatesPublished.Value;

            await _competitionRepo.EditAsync(comp);

            return new { 
                comp.Id, 
                comp.IsPublished, 
                comp.IsRegistrationOpen, 
                comp.IsJuryActive, 
                comp.IsScoreboardLive, 
                comp.IsCertificatesPublished 
            };
        }
    }
}
