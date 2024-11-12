using Unify.Application.Abstractions.Messaging;

namespace Unify.Application.Messages.GetLastMessagesByDate;

public sealed record GetLastMessagesByDateQuery(
    DateTime Date) : IQuery<MessagesResponse>;
