using Application.Modules.Profile.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Profile.Queries.GetMe
{
    public class GetMeQuery : IRequest<MeDto>
    {
    }

    public class GetMeQueryHandler : IRequestHandler<GetMeQuery, MeDto>
    {
        public Task<MeDto> Handle(GetMeQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new MeDto { Id = Guid.NewGuid(), Email = "mock@example.com", Role = "Student" });
        }
    }
}
