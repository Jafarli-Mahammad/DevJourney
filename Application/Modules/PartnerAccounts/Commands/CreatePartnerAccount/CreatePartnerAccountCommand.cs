using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Models.Entities.Jury;
using Application.Services;
using Application.Repositories;

namespace Application.Modules.PartnerAccounts.Commands.CreatePartnerAccount
{
    public class CreatePartnerAccountCommand : IRequest<CreatePartnerAccountDto>
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public Guid CompetitionId { get; set; }
    }

    public class CreatePartnerAccountDto
    {
        public AccountDto Account { get; set; } = new();
        public CredentialsDto Credentials { get; set; } = new();
    }

    public class AccountDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string AvatarInitial { get; set; } = string.Empty;
        public bool HasAccessKey { get; set; }
    }

    public class CredentialsDto
    {
        public string TemporaryPassword { get; set; } = string.Empty;
        public string ReferralCode { get; set; } = string.Empty;
        public string LoginUrl { get; set; } = "https://devjourney.az/login";
    }

    public class CreatePartnerAccountCommandHandler : IRequestHandler<CreatePartnerAccountCommand, CreatePartnerAccountDto>
    {
        private readonly IAuthService _authService;
        private readonly IJuryProfileRepository _juryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePartnerAccountCommandHandler(IAuthService authService, IJuryProfileRepository juryRepository, IUnitOfWork unitOfWork)
        {
            _authService = authService;
            _juryRepository = juryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreatePartnerAccountDto> Handle(CreatePartnerAccountCommand request, CancellationToken cancellationToken)
        {
            var tempPassword = GenerateRandomPassword();
            var userId = await _authService.RegisterAsync(request.Email, request.Email, tempPassword);

            var role = request.Role.ToUpper() == "JURY" ? "JURY" : "SUPPORTER";
            await _authService.AddToRoleAsync(userId, role);

            var referralCode = $"{(role == "JURY" ? "JURY" : "SUPP")}-{new Random().Next(100000, 999999)}";

            if (role == "JURY")
            {
                var juryProfile = new JuryProfile(
                    applicationUserId: userId,
                    juryCode: referralCode,
                    fullName: request.FullName,
                    email: request.Email
                );
                
                juryProfile.Specialization = request.Company; // Abusing Specialization field since company isn't in DB

                await _juryRepository.AddAsync(juryProfile, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            // Mock sending email
            Console.WriteLine($"[MOCK EMAIL] Sent credentials to {request.Email}. Password: {tempPassword}");

            return new CreatePartnerAccountDto
            {
                Account = new AccountDto
                {
                    Id = userId,
                    Name = request.FullName,
                    Email = request.Email,
                    Role = request.Role,
                    Title = role == "JURY" ? "Münsif Heyəti" : "Dəstəkçi",
                    Company = request.Company,
                    AvatarInitial = !string.IsNullOrEmpty(request.FullName) ? request.FullName.Substring(0, 1).ToUpper() : "U",
                    HasAccessKey = true
                },
                Credentials = new CredentialsDto
                {
                    TemporaryPassword = tempPassword,
                    ReferralCode = referralCode,
                    LoginUrl = "https://devjourney.az/login"
                }
            };
        }

        private string GenerateRandomPassword()
        {
            return "X9k#mP2$vL8q"; // Static for testing, should be random in prod
        }
    }
}
