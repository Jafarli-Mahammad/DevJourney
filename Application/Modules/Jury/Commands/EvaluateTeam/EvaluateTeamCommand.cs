using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Jury.Commands.EvaluateTeam
{
    public class EvaluateTeamCommand : IRequest<bool>
    {
        public Guid CompetitionId { get; set; }
        public Guid TeamId { get; set; }
        public List<EvaluationScoreDto> Scores { get; set; } = new();
    }

    public class EvaluationScoreDto
    {
        public Guid CriterionId { get; set; }
        public decimal Score { get; set; }
    }

    public class EvaluateTeamCommandHandler : IRequestHandler<EvaluateTeamCommand, bool>
    {
        public Task<bool> Handle(EvaluateTeamCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
