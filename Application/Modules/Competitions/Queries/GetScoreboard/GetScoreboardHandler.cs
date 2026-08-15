using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Repositories.Competitions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Competitions.Queries.GetScoreboard;

public class GetScoreboardHandler : IRequestHandler<GetScoreboardQuery, List<ScoreboardDto>>
{
    private readonly ICompetitionParticipantRepository _repository;
    private readonly IEvaluationRepository _evaluationRepository;

    public GetScoreboardHandler(ICompetitionParticipantRepository repository, IEvaluationRepository evaluationRepository)
    {
        _repository = repository;
        _evaluationRepository = evaluationRepository;
    }

    public async Task<List<ScoreboardDto>> Handle(GetScoreboardQuery request, CancellationToken cancellationToken)
    {
        var participants = await _repository.GetAllAsync(p => p.CompetitionId == request.CompetitionId, cancellationToken);
        var participantIds = participants.Select(p => p.Id).ToList();
        
        var allEvaluations = await _evaluationRepository.GetAllAsync(e => participantIds.Contains(e.ParticipantId), cancellationToken);
        var evaluationsByParticipant = allEvaluations.ToLookup(e => e.ParticipantId);

        var scoreboard = participants.Select(p =>
        {
            var evaluations = evaluationsByParticipant[p.Id];
            var innovation = evaluations.Sum(e => e.InnovationScore);
            var technical = evaluations.Sum(e => e.TechnicalScore);
            var pitch = evaluations.Sum(e => e.PitchScore);

            return new ScoreboardDto
            {
                ParticipantId = p.Id,
                Name = p.Name,
                InnovationScore = innovation,
                TechnicalScore = technical,
                PitchScore = pitch,
                TotalScore = innovation + technical + pitch
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
