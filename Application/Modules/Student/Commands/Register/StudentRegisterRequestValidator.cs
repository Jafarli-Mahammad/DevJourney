using FluentValidation;

namespace Application.Modules.Student.Commands.Register
{
    public class StudentRegisterRequestValidator : AbstractValidator<StudentRegisterRequest>
    {
        public StudentRegisterRequestValidator()
        {
            RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(250);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(250);

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.")
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.");

            RuleFor(x => x.Age)
                .InclusiveBetween(16, 100).WithMessage("Age must be between 16 and 100.");

            // RuleFor(x => x.Role)
            //     .IsInEnum().WithMessage("Invalid role.");

            RuleFor(x => x.Experience)
                .IsInEnum().WithMessage("Invalid experience level.");

            RuleFor(x => x.PreferredWorkFormat)
                .IsInEnum().WithMessage("Invalid work format.");

            RuleFor(x => x.SkillIds)
                .NotEmpty().WithMessage("At least one skill is required.");

            RuleFor(x => x.Bio)
                .MaximumLength(200).When(x => x.Bio != null);
        }
    }
}