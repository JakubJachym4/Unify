using FluentValidation;
using Unify.Application.Faculty.Commands;

namespace Unify.Application.Faculty.Validators;

public sealed class AddFacultyValidator : AbstractValidator<AddFacultyCommand>
{
    public AddFacultyValidator()
    {
        RuleFor(c => c.Name).NotEmpty();
    }
}