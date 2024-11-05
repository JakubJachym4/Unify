using FluentValidation;
using Unify.Application.Abstractions.Clock;

namespace Unify.Application.Messages.GetLastMessagesByDate;

public class GetLastMessagesByDateValidator : AbstractValidator<GetLastMessagesByDateQuery>
{
    public GetLastMessagesByDateValidator(IDateTimeProvider dateTimeProvider)
    {
        RuleFor(q => q.Date).NotEmpty().LessThan(dateTimeProvider.UtcNowDateOnly);
    }
}