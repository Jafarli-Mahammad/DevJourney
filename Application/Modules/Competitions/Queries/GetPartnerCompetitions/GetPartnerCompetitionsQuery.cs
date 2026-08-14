using System;
using System.Collections.Generic;
using MediatR;

namespace Application.Modules.Competitions.Queries.GetPartnerCompetitions;

public class GetPartnerCompetitionsQuery : IRequest<List<PartnerCompetitionDto>>
{
    public Guid PartnerId { get; set; }
}

public class PartnerCompetitionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public int ApplicantCount { get; set; }
    public int ApprovedCount { get; set; }
    public int CheckInCount { get; set; }
    public int TeamCount { get; set; }
}
