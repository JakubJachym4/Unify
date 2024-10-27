using Unify.Domain.Abstractions;

namespace Unify.Domain.Messages;

public static class MessageErrors
{
    public static Error SenderNotFound = new(
        "Message.SenderNotFound",
        "The message sender with the specified identifier was not found");

    public static Error RecipientNotFound = new(
        "Message.RecipientNotFound",
        "The message recipient with the specified identifier was not found");

}