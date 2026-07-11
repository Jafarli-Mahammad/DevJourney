using FluentValidation;

namespace Application.Modules.Company.Commands.Register
{
    public class CompanyRegisterRequestValidator : AbstractValidator<CompanyRegisterRequest>
    {
        public CompanyRegisterRequestValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Email).NotEmpty(); //.EmailAddress()
            RuleFor(x => x.Password).NotEmpty(); //.MinimumLength(8)

            RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(250);
            RuleFor(x => x.CompanySize).IsInEnum();

            RuleFor(x => x.CompanySector).MaximumLength(150);
            RuleFor(x => x.WebsiteUrl).MaximumLength(300);
            RuleFor(x => x.LinkedInUrl).MaximumLength(300);
            RuleFor(x => x.Location).MaximumLength(150);
            RuleFor(x => x.RepresentativeName).MaximumLength(150);
            RuleFor(x => x.RepresentativeEmail); // .EmailAddress().When(x => !string.IsNullOrEmpty(x.RepresentativeEmail));
        }
    }
}
