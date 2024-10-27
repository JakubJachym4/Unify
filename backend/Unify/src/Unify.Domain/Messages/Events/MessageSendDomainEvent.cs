using MediatR;
using Unify.Domain.Abstractions;
using Unify.Domain.Users;

namespace Unify.Domain.Messages.Events;

public record MessageSendDomainEvent(Guid senderId, IReadOnlyCollection<Guid> recipientsIds) : IDomainEvent;