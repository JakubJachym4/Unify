using FluentValidation;

namespace Unify.Application.FieldsOfStudy;

public sealed class AddFieldOfStudyValidator : AbstractValidator<AddFieldOfStudyCommand>
{
    public AddFieldOfStudyValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).NotNull().NotEmpty().MaximumLength(500);
        RuleFor(x => x.FacultyId).NotEmpty();
    }
}