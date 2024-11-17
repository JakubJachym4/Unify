using FluentValidation;

namespace Unify.Application.Messages.ForwardMessage;

public class ForwardMessageValidator : AbstractValidator<ForwardMessageCommand>
{
    public ForwardMessageValidator()
    {
        RuleFor(c => c.OriginalMessageId).NotEmpty();
        RuleFor(c => c.NewRecipientsIds).NotEmpty();
    }
}