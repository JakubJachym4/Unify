using FluentValidation;
using Unify.Application.Messages.SendMessage;

namespace Unify.Application.Messages.ReplyToMessage;

public class ReplyToMessageCommandValidator : AbstractValidator<ReplyToMessageCommand>
{
    public ReplyToMessageCommandValidator()
    {
        RuleFor(c => c.MessageId).NotEmpty();
        RuleFor(c => c.Title).NotEmpty();
        RuleFor(c => c.Content).NotEmpty();
        RuleFor(c => c.RecipientsIds).NotEmpty();
    }
}