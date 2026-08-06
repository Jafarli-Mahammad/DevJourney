using Application.Repositories;
using Application.Services;
using AutoMapper;
using Domain.Models.Entities.Student;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Student.Commands.Register
{
    public class StudentRegisterRequestHandler : IRequestHandler<StudentRegisterRequest, Guid>
    {
        private readonly IStudentProfileRepository studentProfileRepository;
        private readonly ISkillRepository skillRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILanguageRepository languageRepository;
        private readonly IAuthService authService;

        public StudentRegisterRequestHandler(
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
        
        public async Task<Guid> Handle(StudentRegisterRequest request, CancellationToken cancellationToken)
        {
            using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var userId = await authService.RegisterAsync(
                    request.Email, 
                    request.Email, 
                    request.Password);

                var profile = new StudentProfile(
                    applicationUserId: userId,
                    firstName: request.FirstName,
                    lastName: request.LastName,
                    universityId: request.UniversityId);

                await studentProfileRepository.AddAsync(profile, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return userId;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        /*public async Task<Guid> Handle(StudentRegisterRequest request, CancellationToken cancellationToken)
        {
            using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var userId = await authService.RegisterAsync(
                    request.UserName, request.Email, request.Password);

                var profile = new StudentProfile
                {
                    ApplicationUserId = userId,
                    //Age = request.Age,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Bio = request.Bio,
                    LinkedinUrl = request.LinkedinUrl,
                    GitHubUrl = request.GitHubUrl,
                    ProfessionalRole = request.ProfessionalRole,
                    //Location = request.Location,
                    //Role = request.Role,
                    CVUrl = request.CVUrl,
                    //Experience = request.Experience,
                    //Achievements = request.Achievements,
                    //PreferredWorkFormat = request.PreferredWorkFormat
                };

                /*foreach (var skillId in request.SkillIds ?? [])
                    profile.AddSkill(skillId);

                foreach (var lang in request.Languages ?? [])
                    profile.AddLanguage(lang.LanguageId, lang.ProficiencyLevel);#1#

                await studentProfileRepository.AddAsync(profile, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
                return userId;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }*/
    }
}