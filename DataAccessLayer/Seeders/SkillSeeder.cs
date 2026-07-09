using Application.Seeder;
using DataAccessLayer.DataContexts;
using Domain.Models.Entities.Student;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Seeders
{
    public class SkillSeeder : IDataSeeder
    {
        private readonly DataContext dataContext;

        public SkillSeeder(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task SeedAsync()
        {
            var existingNames = await dataContext.Skills
                .Select(s => s.Name)
                .ToListAsync();

            var skills = new List<Skill>
            {
                // 1. Frontend Development

                new() { Id = Guid.NewGuid(), Name = "TypeScript" },
                new() { Id = Guid.NewGuid(), Name = "React" },
                new() { Id = Guid.NewGuid(), Name = "Angular" },
                new() { Id = Guid.NewGuid(), Name = "Vue.js" },
                new() { Id = Guid.NewGuid(), Name = "Next.js" },
                new() { Id = Guid.NewGuid(), Name = "Tailwind CSS" },
                new() { Id = Guid.NewGuid(), Name = "Bootstrap" },
                new() { Id = Guid.NewGuid(), Name = "Redux / Toolkit" },
                new() { Id = Guid.NewGuid(), Name = "SASS / SCSS" },
                new() { Id = Guid.NewGuid(), Name = "Responsive Web Design" },

                // 2. Backend Development & Languages

                new() { Id = Guid.NewGuid(), Name = "C#" },
                new() { Id = Guid.NewGuid(), Name = ".NET Core / .NET 8" },
                new() { Id = Guid.NewGuid(), Name = "ASP.NET Core Web API" },
                new() { Id = Guid.NewGuid(), Name = "Java" },
                new() { Id = Guid.NewGuid(), Name = "Spring Boot" },
                new() { Id = Guid.NewGuid(), Name = "Python" },
                new() { Id = Guid.NewGuid(), Name = "Django" },
                new() { Id = Guid.NewGuid(), Name = "FastAPI" },
                new() { Id = Guid.NewGuid(), Name = "Node.js" },
                new() { Id = Guid.NewGuid(), Name = "Express.js" },
                new() { Id = Guid.NewGuid(), Name = "Go (Golang)" },
                new() { Id = Guid.NewGuid(), Name = "C++" },
                new() { Id = Guid.NewGuid(), Name = "PHP" },

                // 3. Architecture, Patterns & Concepts

                new() { Id = Guid.NewGuid(), Name = "OOP (Object-Oriented Programming)" },
                new() { Id = Guid.NewGuid(), Name = "SOLID Prinsipləri" },
                new() { Id = Guid.NewGuid(), Name = "Clean Architecture" },
                new() { Id = Guid.NewGuid(), Name = "CQRS Pattern" },
                new() { Id = Guid.NewGuid(), Name = "RESTful APIs" },
                new() { Id = Guid.NewGuid(), Name = "GraphQL" },
                new() { Id = Guid.NewGuid(), Name = "Design Patterns" },
                new() { Id = Guid.NewGuid(), Name = "Microservices" },

                // 4. Databases & ORMs

                new() { Id = Guid.NewGuid(), Name = "Microsoft SQL Server" },
                new() { Id = Guid.NewGuid(), Name = "PostgreSQL" },
                new() { Id = Guid.NewGuid(), Name = "MySQL" },
                new() { Id = Guid.NewGuid(), Name = "MongoDB" },
                new() { Id = Guid.NewGuid(), Name = "Redis" },
                new() { Id = Guid.NewGuid(), Name = "Entity Framework Core" },
                new() { Id = Guid.NewGuid(), Name = "Dapper" },
                new() { Id = Guid.NewGuid(), Name = "Database Design / Normalization" },

                // 5. DevOps, Cloud & Tools

                new() { Id = Guid.NewGuid(), Name = "Git" },
                new() { Id = Guid.NewGuid(), Name = "GitHub / GitLab" },
                new() { Id = Guid.NewGuid(), Name = "Docker" },
                new() { Id = Guid.NewGuid(), Name = "Kubernetes" },
                new() { Id = Guid.NewGuid(), Name = "Linux / Bash" },
                new() { Id = Guid.NewGuid(), Name = "AWS (Amazon Web Services)" },
                new() { Id = Guid.NewGuid(), Name = "Microsoft Azure" },
                new() { Id = Guid.NewGuid(), Name = "CI/CD Pipelines" },

                // 6. Mobile & Desktop Development

                new() { Id = Guid.NewGuid(), Name = "Flutter" },
                new() { Id = Guid.NewGuid(), Name = "React Native" },
                new() { Id = Guid.NewGuid(), Name = "Kotlin" },
                new() { Id = Guid.NewGuid(), Name = "Swift" },
                new() { Id = Guid.NewGuid(), Name = ".NET MAUI" },

                // 7. Data Science, QA & AI

                new() { Id = Guid.NewGuid(), Name = "Machine Learning" },
                new() { Id = Guid.NewGuid(), Name = "Data Analysis" },
                new() { Id = Guid.NewGuid(), Name = "Pandas / NumPy" },
                new() { Id = Guid.NewGuid(), Name = "Prompt Engineering" },
                new() { Id = Guid.NewGuid(), Name = "AI API Integration" },
                new() { Id = Guid.NewGuid(), Name = "Unit Testing (xUnit / NUnit)" },
                new() { Id = Guid.NewGuid(), Name = "Manual Testing" },
                new() { Id = Guid.NewGuid(), Name = "Automation Testing (Selenium)" },

                // 8. UI/UX & Design

                new() { Id = Guid.NewGuid(), Name = "Figma" },
                new() { Id = Guid.NewGuid(), Name = "UI/UX Design" },
                new() { Id = Guid.NewGuid(), Name = "Adobe Photoshop" },
                new() { Id = Guid.NewGuid(), Name = "Adobe Illustrator" },
                new() { Id = Guid.NewGuid(), Name = "Prototyping" },
                new() { Id = Guid.NewGuid(), Name = "Wireframing" },

                // 9. Management, Soft Skills & Universitet Həyatı

                new() { Id = Guid.NewGuid(), Name = "Agile / Scrum" },
                new() { Id = Guid.NewGuid(), Name = "Teamwork / Komanda işi" },
                new() { Id = Guid.NewGuid(), Name = "Problem Solving" },
                new() { Id = Guid.NewGuid(), Name = "Public Speaking / Təqdimat bacarığı" },
                new() { Id = Guid.NewGuid(), Name = "Technical Writing / Texniki Sənədləşdirmə" },
                new() { Id = Guid.NewGuid(), Name = "Project Management" },
                new() { Id = Guid.NewGuid(), Name = "Time Management" },
                new() { Id = Guid.NewGuid(), Name = "Critical Thinking" },
                new() { Id = Guid.NewGuid(), Name = "Algorithmic Thinking" },
                new() { Id = Guid.NewGuid(), Name = "Data Structures & Algorithms" },
            };

            var newSkills = skills
                .Where(s => !existingNames.Contains(s.Name))
                .ToList();

            if (!newSkills.Any()) return;

            await dataContext.Skills.AddRangeAsync(newSkills);
            await dataContext.SaveChangesAsync();
        }
    }
}
