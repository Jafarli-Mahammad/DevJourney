using Application.Exceptions;
using Application.Repositories;
using Application.Repositories.Competitions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Competitions.Commands.DeleteCompetition
{
    public class DeleteCompetitionCommand : IRequest<bool>
    {
        public Guid CompetitionId { get; set; }
    }

    public class DeleteCompetitionCommandHandler : IRequestHandler<DeleteCompetitionCommand, bool>
    {
        private readonly ICompetitionRepository _competitionRepo;
        public DeleteCompetitionCommandHandler(ICompetitionRepository competitionRepo)
        {
            _competitionRepo = competitionRepo;
        }

        public async Task<bool> Handle(DeleteCompetitionCommand request, CancellationToken cancellationToken)
        {
            var comp = await _competitionRepo.GetAsync(c => c.Id == request.CompetitionId, null, cancellationToken);
            if (comp == null) throw new NotFoundException("Competition", request.CompetitionId);
            
            _competitionRepo.Remove(comp);
            return true;
        }
    }
}
