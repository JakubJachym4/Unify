using Unify.Application.Abstractions.Messaging;

namespace Unify.Application.Messages.GetSentMessages;

public sealed record GetSentMessagesQuery(Guid UserId) : IQuery<MessagesResponse>;