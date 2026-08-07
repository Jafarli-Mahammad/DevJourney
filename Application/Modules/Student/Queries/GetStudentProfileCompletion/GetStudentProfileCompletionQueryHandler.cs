using Application.Repositories;
using MediatR;

namespace Application.Modules.Student.Queries.GetStudentProfileCompletion
{
    public class GetStudentProfileCompletionQueryHandler : IRequestHandler<GetStudentProfileCompletionQuery, ProfileCompletionDto?>
    {
        private readonly IStudentProfileRepository _studentProfileRepository;

        public GetStudentProfileCompletionQueryHandler(IStudentProfileRepository studentProfileRepository)
        {
            _studentProfileRepository = studentProfileRepository;
        }

        public async Task<ProfileCompletionDto?> Handle(GetStudentProfileCompletionQuery request, CancellationToken cancellationToken)
        {
            var profile = await _studentProfileRepository.GetFullProfileByIdAsync(request.StudentId, cancellationToken);
            if (profile == null)
            {
                return null;
            }

            return new ProfileCompletionDto
            {
                StudentProfileId = profile.Id,
                CompletionPercentage = profile.CalculateProfileCompletionPercentage()
            };
        }
    }
}
