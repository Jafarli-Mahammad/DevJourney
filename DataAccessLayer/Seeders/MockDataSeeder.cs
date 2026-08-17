using Application.Seeder;
using Bogus;
using DataAccessLayer.DataContexts;
using DataAccessLayer.IdentityEntities;
using Domain.Models.Entities.Competition;
using Domain.Models.Entities.Partner;
using Domain.Models.Entities.Student;
using Domain.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Seeders
{
    public class MockDataSeeder : IDataSeeder
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public MockDataSeeder(DataContext dataContext, UserManager<ApplicationUser> userManager)
        {
            _dataContext = dataContext;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            // Only seed if there are no students (meaning DB is relatively empty of mock data)
            if (await _dataContext.StudentProfiles.AnyAsync())
            {
                return;
            }

            var password = "Password123!";

            // 1. Seed Partners
            var partners = new List<PartnerProfile>();
            var partnerFaker = new Faker<PartnerProfile>()
                .RuleFor(p => p.PartnerName, f => f.Company.CompanyName())
                .RuleFor(p => p.PartnerType, f => f.PickRandom<PartnerType>())
                .RuleFor(p => p.WebsiteUrl, f => f.Internet.Url())
                .RuleFor(p => p.Location, f => f.Address.City())
                .RuleFor(p => p.Description, f => f.Company.CatchPhrase())
                .RuleFor(p => p.IsVerified, f => true);

            for (int i = 0; i < 5; i++)
            {
                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = $"partner{i}@test.com",
                    Email = $"partner{i}@test.com",
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    var partner = partnerFaker.Generate();
                    partner.ApplicationUserId = user.Id;
                    partners.Add(partner);
                }
            }
            await _dataContext.PartnerProfiles.AddRangeAsync(partners);
            await _dataContext.SaveChangesAsync();

            // 2. Seed Students
            var universities = await _dataContext.UniversityProfiles.ToListAsync();
            var students = new List<StudentProfile>();
            var studentFaker = new Faker<StudentProfile>()
                .CustomInstantiator(f => new StudentProfile(
                    Guid.Empty,
                    f.Name.FirstName(),
                    f.Name.LastName(),
                    universities.Any() ? f.PickRandom(universities).Id : null
                ))
                .RuleFor(s => s.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(s => s.Course, f => f.PickRandom(new[] { "BSc Computer Science", "BEng Software Engineering", "IT", "Information Systems" }))
                .RuleFor(s => s.GitHubUrl, f => $"https://github.com/{f.Internet.UserName()}")
                .RuleFor(s => s.LinkedinUrl, f => $"https://linkedin.com/in/{f.Internet.UserName()}")
                .RuleFor(s => s.ExperienceLevel, f => f.PickRandom<ExperienceLevel>())
                .RuleFor(s => s.Bio, f => f.Lorem.Paragraph());

            for (int i = 0; i < 50; i++)
            {
                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = $"student{i}@test.com",
                    Email = $"student{i}@test.com",
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    var student = studentFaker.Generate();
                    student.ApplicationUserId = user.Id;
                    students.Add(student);
                }
            }
            await _dataContext.StudentProfiles.AddRangeAsync(students);
            await _dataContext.SaveChangesAsync();

            // 3. Seed Competitions
            var competitions = new List<Competition>();
            var competitionFaker = new Faker<Competition>()
                .RuleFor(c => c.PartnerId, f => f.PickRandom(partners).Id)
                .RuleFor(c => c.Title, f => f.Commerce.ProductName() + " Hackathon")
                .RuleFor(c => c.ShortSummary, f => f.Lorem.Sentence())
                .RuleFor(c => c.Description, f => f.Lorem.Paragraphs(3))
                .RuleFor(c => c.ParticipationFormat, f => f.PickRandom<ParticipationFormat>())
                .RuleFor(c => c.MaxTeamSize, f => f.Random.Int(2, 5))
                .RuleFor(c => c.StartDate, f => f.Date.Soon(30))
                .RuleFor(c => c.EndDate, (f, c) => c.StartDate.AddDays(f.Random.Int(2, 5)))
                .RuleFor(c => c.RegistrationDeadline, (f, c) => c.StartDate.AddDays(-2))
                .RuleFor(c => c.Location, f => f.Address.FullAddress())
                .RuleFor(c => c.IsPublished, f => true);

            for (int i = 0; i < 10; i++)
            {
                competitions.Add(competitionFaker.Generate());
            }
            await _dataContext.Competitions.AddRangeAsync(competitions);
            await _dataContext.SaveChangesAsync();

            // 4. Seed Competition Participants (Teams & Individuals)
            var participants = new List<CompetitionParticipant>();
            var participantFaker = new Faker<CompetitionParticipant>()
                .RuleFor(p => p.CompetitionId, f => f.PickRandom(competitions).Id)
                .RuleFor(p => p.Name, f => f.Company.CatchPhrase() + " Team")
                .RuleFor(p => p.IsTeam, f => true)
                .RuleFor(p => p.AppliedAt, f => f.Date.Recent(10))
                .RuleFor(p => p.Status, f => f.PickRandom<ApplicationStatus>())
                .RuleFor(p => p.ProjectName, f => f.Commerce.ProductName())
                .RuleFor(p => p.ProjectDescription, f => f.Lorem.Sentence());

            for (int i = 0; i < 20; i++)
            {
                var team = participantFaker.Generate();
                
                // Pick random students for the team
                var teamSize = new Random().Next(2, 5);
                var shuffledStudents = students.OrderBy(x => Guid.NewGuid()).Take(teamSize).ToList();
                
                team.CaptainId = shuffledStudents.First().Id;
                
                foreach (var s in shuffledStudents)
                {
                    team.Members.Add(new CompetitionTeamMember
                    {
                        StudentProfileId = s.Id,
                        Role = s.Id == team.CaptainId ? "Captain" : "Member"
                    });
                }
                participants.Add(team);
            }
            
            await _dataContext.CompetitionParticipants.AddRangeAsync(participants);
            await _dataContext.SaveChangesAsync();
        }
    }
}
