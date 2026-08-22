using System.Threading;
using System.Threading.Tasks;
using Application.Repositories;
using Application.Repositories.Competitions;


using MediatR;

namespace Application.Modules.Competitions.Commands.UpdateApplicationStatus;

public class UpdateApplicationStatusHandler : IRequestHandler<UpdateApplicationStatusCommand, bool>
{
    private readonly ICompetitionParticipantRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateApplicationStatusHandler(ICompetitionParticipantRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateApplicationStatusCommand request, CancellationToken cancellationToken)
    {
        var participant = await _repository.GetAsync(p => p.Id == request.ParticipantId, null, cancellationToken);
        
        if (participant == null)
            return false;

        participant.Status = request.Status;
        await _repository.EditAsync(participant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
