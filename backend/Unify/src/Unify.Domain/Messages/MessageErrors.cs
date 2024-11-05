using Unify.Domain.Abstractions;

namespace Unify.Domain.Messages;

public static class MessageErrors
{
    public static Error SenderNotFound = new(
        "Message.SenderNotFound",
        "The message sender with the specified identifier was not found");


    public static Error RecipientNotFound(Guid id) =>
        Error.Create("Message.Recipient not found",
            "The message recipient with the specified identifier was not found. Id: {0}",
            id);



    public static Error NotFound(Guid id) =>
        Error.Create("Message.NotFound",
            "The message with the specified identifier was not found. Id: {0}",
            id);

}