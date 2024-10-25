using Unify.Domain.Abstractions;

namespace Unify.Domain.Users.Events;

public sealed record UserCreatedDomainEvent(Guid UserId) : IDomainEvent;