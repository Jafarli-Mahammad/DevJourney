using Application.Repositories;

namespace Application.Modules.Student.Commands.Verify;

public class VerifyStudentUniversityHandler
{
    private readonly IStudentProfileRepository studentProfileRepository;
    private readonly IUniversityProfileRepository universityProfileRepository;

    public async Task<bool> Handle(VerifyStudentUniversityRequest request, CancellationToken cancellationToken)
    {
        var student = await studentProfileRepository.GetByIdAsync(request.StudentProfileId);
        if (student?.UniversityId == null)
            return false;

        var university = await universityProfileRepository.GetAsync(u => u.Id == student.UniversityId);
        if (university == null)
            return false;

        // Verify email domain matches, or whatever your logic is
        //var emailDomain = student.Email.Split('@')[1]; // Or get from ApplicationUser
        //return university.EmailDomain == emailDomain; // Or your verification logic
        return true;
    }
}