using FluentValidation;
using Unify.Application.StudentGroups.CommandsAndQueries;

namespace Unify.Application.StudentGroups.Validators;

public class CreateStudentGroupValidator : AbstractValidator<CreateStudentGroupCommand>
{
    public CreateStudentGroupValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(10);

        RuleFor(x => x.SpecializationId)
            .NotEmpty();

        RuleFor(x => x.StudyYear)
            .NotEmpty();

        RuleFor(x => x.Semester)
            .NotEmpty();

        RuleFor(x => x.Term)
            .NotEmpty();

        RuleFor(x => x.MaxGroupSize)
            .NotEmpty();
    }
}

public class CreateStudentGroupsForSpecializationValidator : AbstractValidator<CreateStudentGroupsForSpecializationCommand>
{
    public CreateStudentGroupsForSpecializationValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(10);

        RuleFor(x => x.SpecializationId)
            .NotEmpty();

        RuleFor(x => x.StudyYear)
            .NotEmpty();

        RuleFor(x => x.Semester)
            .NotEmpty();

        RuleFor(x => x.Term)
            .NotEmpty();

        RuleFor(x => x.MaxGroupSize)
            .NotEmpty();
    }
}