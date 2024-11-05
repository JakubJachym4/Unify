using Unify.Application.Abstractions.Messaging;

namespace Unify.Application.Messages.GetLastMessagesByNumber;

public sealed record GetLastMessagesByNumberQuery(
    int NumberOfMessages) : IQuery<MessagesResponse>;
