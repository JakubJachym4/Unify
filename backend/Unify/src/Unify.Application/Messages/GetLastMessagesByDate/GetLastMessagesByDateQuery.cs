using Unify.Application.Abstractions.Messaging;

namespace Unify.Application.Messages.GetLastMessagesByDate;

public sealed record GetLastMessagesByDateQuery(
    DateOnly Date) : IQuery<MessagesResponse>;
