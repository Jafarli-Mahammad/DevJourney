using FluentValidation;

namespace Application.Modules.University.Commands.Register
{
    public class UniversityRegisterRequestValidator: AbstractValidator<UniversityRegisterRequest>
    {
        public UniversityRegisterRequestValidator()
        {
            RuleFor(x => x.UserName).NotEmpty();
            RuleFor(x => x.Email).NotEmpty(); //.EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
            RuleFor(x => x.UniversityName).NotEmpty();

            // Optional fields - only validate format if they actually provide them
            RuleFor(x => x.RepresentativeEmail); //                .EmailAddress().When(x => !string.IsNullOrEmpty(x.RepresentativeEmail));
        }
    }
}