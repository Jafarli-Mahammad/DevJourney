using FluentValidation;
using System;

namespace Application.Modules.Competitions.Commands.CreateCompetition
{
    public class CreateCompetitionCommandValidator : AbstractValidator<CreateCompetitionCommand>
    {
        public CreateCompetitionCommandValidator()
        {
            RuleFor(x => x.Dto).NotNull();

            When(x => x.Dto != null, () =>
            {
                RuleFor(x => x.Dto.Title)
                    .NotEmpty().WithMessage("Title is required.")
                    .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

                RuleFor(x => x.Dto.Description)
                    .NotEmpty().WithMessage("Description is required.");

                RuleFor(x => x.Dto.StartDate)
                    .NotEmpty().WithMessage("Start date is required.");

                RuleFor(x => x.Dto.EndDate)
                    .NotEmpty().WithMessage("End date is required.")
                    .GreaterThanOrEqualTo(x => x.Dto.StartDate).WithMessage("End date must be greater than or equal to start date.");

                RuleFor(x => x.Dto.RegistrationDeadline)
                    .NotEmpty().WithMessage("Registration deadline is required.")
                    .LessThanOrEqualTo(x => x.Dto.StartDate).WithMessage("Registration deadline must be before or on the start date.");

                RuleFor(x => x.Dto.SubmissionDeadline)
                    .NotEmpty().WithMessage("Submission deadline is required.")
                    .GreaterThanOrEqualTo(x => x.Dto.StartDate).WithMessage("Submission deadline must be after or on the start date.")
                    .LessThanOrEqualTo(x => x.Dto.EndDate).WithMessage("Submission deadline must be before or on the end date.");

                RuleFor(x => x.Dto.MaxTeamSize)
                    .GreaterThan(0).WithMessage("Max team size must be greater than 0.");

                RuleFor(x => x.Dto.ContactEmail)
                    .EmailAddress().When(x => !string.IsNullOrEmpty(x.Dto.ContactEmail)).WithMessage("A valid email address is required.");

                RuleForEach(x => x.Dto.Stages).ChildRules(stages =>
                {
                    stages.RuleFor(s => s.Title).NotEmpty().WithMessage("Stage title is required.");
                    stages.RuleFor(s => s.DayNumber).GreaterThan(0).WithMessage("Day number must be greater than 0.");
                    stages.RuleFor(s => s.EndTime).GreaterThanOrEqualTo(s => s.StartTime).WithMessage("Stage end time must be after start time.");
                });
            });
        }
    }
}
