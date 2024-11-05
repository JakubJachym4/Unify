using FluentValidation;
using Unify.Application.Messages.GetLastMessagesByDate;

namespace Unify.Application.Messages.GetLastMessagesByNumber;

public class GetLastMessagesByNumberValidator : AbstractValidator<GetLastMessagesByNumberQuery>
{
    public GetLastMessagesByNumberValidator()
    {
        RuleFor(q => q.NumberOfMessages).NotEmpty().GreaterThan(0);
    }
}