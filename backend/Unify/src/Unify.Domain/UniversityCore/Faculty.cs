using Unify.Domain.Abstractions;
using Unify.Domain.Shared;

namespace Unify.Domain.UniversityCore;

public sealed class Faculty : Entity
{
    public Name Name { get; private set; }
}