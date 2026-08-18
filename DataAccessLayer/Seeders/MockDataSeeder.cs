using Application.Seeder;
using Bogus;
using DataAccessLayer.DataContexts;
using DataAccessLayer.IdentityEntities;
using Domain.Models.Entities.Competition;
using Domain.Models.Entities.Core;
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
            var password = "Password123!";

            // 1. Ensure Partners Exist
            var partners = await _dataContext.PartnerProfiles.ToListAsync();
            if (partners.Count < 5)
            {
                var partnerFaker = new Faker<PartnerProfile>()
                    .RuleFor(p => p.PartnerName, f => f.Company.CompanyName())
                    .RuleFor(p => p.PartnerType, f => f.PickRandom<PartnerType>())
                    .RuleFor(p => p.WebsiteUrl, f => f.Internet.Url())
                    .RuleFor(p => p.Location, f => f.Address.City())
                    .RuleFor(p => p.Description, f => f.Company.CatchPhrase())
                    .RuleFor(p => p.IsVerified, f => true);

                for (int i = 0; i < 5 - partners.Count; i++)
                {
                    var user = new ApplicationUser
                    {
                        Id = Guid.NewGuid(),
                        UserName = $"partner_mock_{i}_{Guid.NewGuid().ToString().Substring(0, 5)}@test.com",
                        Email = $"partner_mock_{i}_{Guid.NewGuid().ToString().Substring(0, 5)}@test.com",
                        EmailConfirmed = true
                    };
                    var result = await _userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        var partner = partnerFaker.Generate();
                        partner.ApplicationUserId = user.Id;
                        _dataContext.PartnerProfiles.Add(partner);
                        partners.Add(partner);
                    }
                }
                await _dataContext.SaveChangesAsync();
            }

            // 2. Ensure Competitions Exist
            var competitions = await _dataContext.Competitions.ToListAsync();
            if (competitions.Count < 10)
            {
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

                for (int i = 0; i < 10 - competitions.Count; i++)
                {
                    var comp = competitionFaker.Generate();
                    _dataContext.Competitions.Add(comp);
                    competitions.Add(comp);
                }
                await _dataContext.SaveChangesAsync();
            }

            // 3. Ensure Students Exist
            var universities = await _dataContext.UniversityProfiles.ToListAsync();
            var students = await _dataContext.StudentProfiles.ToListAsync();
            if (students.Count < 40)
            {
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

                for (int i = 0; i < 40 - students.Count; i++)
                {
                    var user = new ApplicationUser
                    {
                        Id = Guid.NewGuid(),
                        UserName = $"student_mock_{i}_{Guid.NewGuid().ToString().Substring(0, 5)}@test.com",
                        Email = $"student_mock_{i}_{Guid.NewGuid().ToString().Substring(0, 5)}@test.com",
                        EmailConfirmed = true
                    };
                    var result = await _userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        var student = studentFaker.Generate();
                        student.ApplicationUserId = user.Id;
                        _dataContext.StudentProfiles.Add(student);
                        students.Add(student);
                    }
                }
                await _dataContext.SaveChangesAsync();
            }

            // 3.5 Ensure Demo Accounts Exist and are Populated
            var demoAccounts = new[] 
            { 
                new { Email = "demo.active@devjourney.az", NeedsData = true }, 
                new { Email = "demo.new@devjourney.az", NeedsData = false } 
            };
            var demoPassword = "Demo1234";

            var hasher = new PasswordHasher<ApplicationUser>();
            foreach (var demo in demoAccounts)
            {
                var demoUser = await _userManager.FindByEmailAsync(demo.Email);
                if (demoUser == null)
                {
                    demoUser = new ApplicationUser
                    {
                        Id = Guid.NewGuid(),
                        UserName = demo.Email,
                        Email = demo.Email,
                        EmailConfirmed = true
                    };
                    demoUser.PasswordHash = hasher.HashPassword(demoUser, demoPassword);
                    await _userManager.CreateAsync(demoUser);
                }
                else
                {
                    // Force reset password to Demo1234, bypassing password policy validators
                    demoUser.PasswordHash = hasher.HashPassword(demoUser, demoPassword);
                    await _userManager.UpdateAsync(demoUser);
                }

                var student = await _dataContext.StudentProfiles.FirstOrDefaultAsync(s => s.ApplicationUserId == demoUser.Id);
                if (student == null)
                {
                    student = new StudentProfile(
                        Guid.Empty,
                        demo.NeedsData ? "Demo" : "New",
                        demo.NeedsData ? "ActiveUser" : "User",
                        universities.FirstOrDefault()?.Id
                    )
                    {
                        ApplicationUserId = demoUser.Id,
                        PhoneNumber = "+994500000000",
                        Course = "BSc Computer Science",
                        ExperienceLevel = ExperienceLevel.Middle,
                        Bio = "This is a demo account automatically seeded for testing purposes."
                    };
                    _dataContext.StudentProfiles.Add(student);
                    students.Add(student);
                    await _dataContext.SaveChangesAsync();
                }

                if (demo.NeedsData)
                {
                    // Find their student profile ID
                    var profile = await _dataContext.StudentProfiles.FirstOrDefaultAsync(s => s.ApplicationUserId == demoUser.Id);
                    if (profile != null)
                    {
                        // Check if they already have certificates
                        var hasData = await _dataContext.Certificates.AnyAsync(c => c.UserId == demoUser.Id);
                        if (!hasData)
                        {
                            var demoFaker = new Faker();
                            var shuffledComps = competitions.OrderBy(x => Guid.NewGuid()).Take(4).ToList();
                            
                            foreach (var comp in shuffledComps)
                            {
                                var team = new CompetitionParticipant
                                {
                                    CompetitionId = comp.Id,
                                    Name = "Demo Active Team " + demoFaker.Random.Int(1, 100),
                                    IsTeam = true,
                                    AppliedAt = demoFaker.Date.Recent(30),
                                    Status = ApplicationStatus.Approved,
                                    ProjectName = "Demo Innovation Project",
                                    ProjectDescription = "A revolutionary product made by the demo user.",
                                    CaptainId = profile.Id
                                };
                                team.Members.Add(new CompetitionTeamMember { StudentProfileId = profile.Id, Role = "Captain" });
                                _dataContext.CompetitionParticipants.Add(team);

                                _dataContext.Certificates.Add(new Certificate
                                {
                                    UserId = demoUser.Id,
                                    Title = demoFaker.PickRandom(new[] { "1st Place Winner", "Best Innovation", "Outstanding Pitch", "Hackathon Champion" }) + " - " + comp.Title,
                                    Description = "Awarded for exceptional performance in the " + comp.Title,
                                    AssetId = "certificates/mock-cert-" + demoFaker.Random.Int(1, 5) + ".svg"
                                });
                            }
                            await _dataContext.SaveChangesAsync();
                        }
                    }
                }
            }

            // 4. Attach Competitions and Certificates to EVERY Student
            var faker = new Faker();
            var allCertificates = await _dataContext.Certificates.ToListAsync();
            var allParticipants = await _dataContext.CompetitionParticipants.Include(cp => cp.Members).ToListAsync();

            foreach (var student in students)
            {
                // Attach exactly 2 certificates
                int certCount = 2;
                for (int i = 0; i < certCount; i++)
                {
                    _dataContext.Certificates.Add(new Certificate
                    {
                        UserId = student.ApplicationUserId,
                        Title = faker.PickRandom(new[] { "1st Place Winner", "Top 10 Finalist", "Participation Award", "Best UI/UX Award", "Most Innovative" }) + " - " + faker.Company.CatchPhrase(),
                        Description = faker.Lorem.Sentence(),
                        AssetId = "certificates/mock-cert-" + faker.Random.Int(1, 5) + ".svg"
                    });
                }

                // Attach to random competition
                var comp = faker.PickRandom(competitions);
                var team = new CompetitionParticipant
                {
                    CompetitionId = comp.Id,
                    Name = faker.Commerce.ProductName() + " Team",
                    IsTeam = true,
                    AppliedAt = faker.Date.Recent(30),
                    Status = ApplicationStatus.Approved,
                    ProjectName = faker.Commerce.ProductName(),
                    ProjectDescription = faker.Lorem.Sentence(),
                    CaptainId = student.Id
                };
                
                team.Members.Add(new CompetitionTeamMember
                {
                    StudentProfileId = student.Id,
                    Role = "Captain"
                });
                
                _dataContext.CompetitionParticipants.Add(team);
                allParticipants.Add(team);
            }

            await _dataContext.SaveChangesAsync();
        }
    }
}
