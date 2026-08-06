using Application.Exceptions;
using Application.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Student.Queries.GetStudentProfile
{
    public class GetStudentProfileQueryHandler : IRequestHandler<GetStudentProfileQuery, StudentProfileDto>
    {
        private readonly IStudentProfileRepository _studentProfileRepository;

        public GetStudentProfileQueryHandler(IStudentProfileRepository studentProfileRepository)
        {
            _studentProfileRepository = studentProfileRepository;
        }

        public async Task<StudentProfileDto> Handle(GetStudentProfileQuery request, CancellationToken cancellationToken)
        {
            var data = await _studentProfileRepository.GetWithEmailByIdAsync(request.Id, cancellationToken);
            if (data == null)
            {
                return null!;
            }

            return new StudentProfileDto
            {
                Id = data.Value.Profile.Id,
                ApplicationUserId = data.Value.Profile.ApplicationUserId,
                Email = data.Value.Email,
                FirstName = data.Value.Profile.FirstName,
                LastName = data.Value.Profile.LastName,
                UniversityId = data.Value.Profile.UniversityId,
                Bio = data.Value.Profile.Bio,
                ProfessionalRole = data.Value.Profile.ProfessionalRole,
                GitHubUrl = data.Value.Profile.GitHubUrl,
                LinkedinUrl = data.Value.Profile.LinkedinUrl,
                CVUrl = data.Value.Profile.CVUrl
            };
        }
    }
}
