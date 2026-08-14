using Application.Repositories;
using Domain.Models.Entities.Competition;

namespace Application.Repositories.Competitions;

public interface ICompetitionParticipantRepository : IAsyncRepository<CompetitionParticipant>
{
}
