using Application.Repositories;
using Application.Services;
using AutoMapper;
using Domain.Models.Entities.Student;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Student.Commands.Register
{
    public class RegisterStudentCommandHandler : IRequestHandler<RegisterStudentCommand, Guid>
    {
        private readonly IStudentProfileRepository studentProfileRepository;
        private readonly ISkillRepository skillRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILanguageRepository languageRepository;
        private readonly IAuthService authService;

        public RegisterStudentCommandHandler( 
            IStudentProfileRepository studentProfileRepository,
            ISkillRepository skillRepository,
            IUnitOfWork unitOfWork,
            ILanguageRepository languageRepository,
            IAuthService authService)
        {
            this.studentProfileRepository = studentProfileRepository;
            this.skillRepository = skillRepository;
            this.unitOfWork = unitOfWork;
            this.languageRepository = languageRepository;
            this.authService = authService;
        }

        public async Task<Guid> Handle(RegisterStudentCommand request, CancellationToken cancellationToken)
        {
            var userId = await authService.RegisterAsync(request.FirstName, request.LastName, request.UserName, request.Email, request.Password);

            var profile = new StudentProfile
            {
                ApplicationUserId = userId,
                Age = request.Age,
                Location = request.Location,
                Role = request.Role,
                CVUrl = request.CVUrl,
                LinkedinUrl = request.LinkedinUrl,
                GitHubUrl = request.GitHubUrl,
                Experience = request.Experience,
                Achievements = request.Achievements,
                Bio = request.Bio,
                PreferredWorkFormat = request.PreferredWorkFormat
            };

            if (request.SkillIds?.Any() == true)
            {
                profile.StudentSkills = request.SkillIds.Select(skillId => new StudentSkill
                {
                    SkillId = skillId,
                    StudentProfileId = profile.Id
                }).ToList();
            }

            if (request.Languages?.Any() == true)
            {
                profile.StudentLanguages = request.Languages.Select(l => new StudentLanguage
                {
                    LanguageId = l.LanguageId,
                    ProficiencyLevel = l.ProficiencyLevel,
                    StudentProfileId = profile.Id
                }).ToList();
            }

            await studentProfileRepository.AddAsync(profile, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return userId;
        }
    }
}
