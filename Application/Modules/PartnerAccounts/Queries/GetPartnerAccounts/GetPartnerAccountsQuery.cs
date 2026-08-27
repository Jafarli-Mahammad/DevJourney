using Application.Repositories;

using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.PartnerAccounts.Queries.GetPartnerAccounts
{
    public class PartnerAccountMemberDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = "Münsif Heyəti";
        public string Company { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string AvatarInitial { get; set; } = string.Empty;
        public string Role { get; set; } = "jury";
        public bool HasAccessKey { get; set; } = true;
        public string? ReferralCode { get; set; }
    }

    public class GetPartnerAccountsQuery : IRequest<List<PartnerAccountMemberDto>>
    {
    }

    public class GetPartnerAccountsQueryHandler : IRequestHandler<GetPartnerAccountsQuery, List<PartnerAccountMemberDto>>
    {
        private readonly IJuryProfileRepository _juryProfileRepository;

        public GetPartnerAccountsQueryHandler(IJuryProfileRepository juryProfileRepository)
        {
            _juryProfileRepository = juryProfileRepository;
        }

        public async Task<List<PartnerAccountMemberDto>> Handle(GetPartnerAccountsQuery request, CancellationToken cancellationToken)
        {
            var juries = await _juryProfileRepository.GetAllAsync(null, cancellationToken);
            return juries.Select(j => new PartnerAccountMemberDto
            {
                Id = j.Id,
                Name = j.FullName ?? j.JuryCode,
                Title = "Münsif Heyəti",
                Company = j.Specialization ?? "DevJourney",
                Email = j.Email ?? string.Empty,
                AvatarInitial = !string.IsNullOrEmpty(j.FullName) ? j.FullName.Substring(0, 1).ToUpper() : "M",
                Role = "jury",
                HasAccessKey = true,
                ReferralCode = j.JuryCode
            }).ToList();
        }
    }
}
