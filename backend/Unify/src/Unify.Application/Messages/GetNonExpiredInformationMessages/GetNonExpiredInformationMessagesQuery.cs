using Unify.Application.Abstractions.Messaging;

namespace Unify.Application.Messages.GetNonExpiredInformationMessages;

public sealed record GetNonExpiredInformationMessagesQuery() : IQuery<InformationMessagesResponse>;