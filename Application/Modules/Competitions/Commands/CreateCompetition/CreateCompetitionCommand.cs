using Application.Modules.Competitions.Dtos;
using MediatR;
using System;

namespace Application.Modules.Competitions.Commands.CreateCompetition
{
    public class CreateCompetitionCommand : IRequest<Guid>
    {
        public CreateCompetitionDto Dto { get; set; } = null!;
        public Guid PartnerId { get; set; } 
    }
}
