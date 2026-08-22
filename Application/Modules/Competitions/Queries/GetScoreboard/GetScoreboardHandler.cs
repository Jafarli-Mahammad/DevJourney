using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;
using Application.Repositories.Competitions;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Competitions.Queries.GetScoreboard;

public class GetScoreboardHandler : IRequestHandler<GetScoreboardQuery, List<ScoreboardDto>>
{
    private readonly ICompetitionParticipantRepository _repository;
    private readonly IEvaluationRepository _evaluationRepository;
    private readonly ICompetitionRepository _competitionRepository;

    public GetScoreboardHandler(ICompetitionParticipantRepository repository, IEvaluationRepository evaluationRepository, ICompetitionRepository competitionRepository)
    {
        _repository = repository;
        _evaluationRepository = evaluationRepository;
        _competitionRepository = competitionRepository;
    }

    public async Task<List<ScoreboardDto>> Handle(GetScoreboardQuery request, CancellationToken cancellationToken)
    {
        var competition = await _competitionRepository.GetAsync(c => c.Id == request.CompetitionId, null, cancellationToken);
        if (competition == null)
            throw new Application.Exceptions.NotFoundException("Competition", request.CompetitionId);

        var participants = await _repository.GetAllAsync(p => p.CompetitionId == request.CompetitionId, cancellationToken);
        var participantIds = participants.Select(p => p.Id).ToList();
        
        var allEvaluations = await _evaluationRepository.GetAllAsync(e => participantIds.Contains(e.ParticipantId), cancellationToken);
        var evaluationsByParticipant = allEvaluations.ToLookup(e => e.ParticipantId);

        var scoreboard = participants.Select(p =>
        {
            var evaluations = evaluationsByParticipant[p.Id];
            var total = evaluations.Sum(e => e.Score);

            return new ScoreboardDto
            {
                ParticipantId = p.Id,
                Name = p.Name,
                TotalScore = total
            };
        })
        .OrderByDescending(s => s.TotalScore)
        .ToList();

        for (int i = 0; i < scoreboard.Count; i++)
        {
            scoreboard[i].Rank = i + 1;
        }

        return scoreboard;
    }
}
