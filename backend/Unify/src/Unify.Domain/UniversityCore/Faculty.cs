using Unify.Domain.Abstractions;
using Unify.Domain.Shared;

namespace Unify.Domain.UniversityCore;

public sealed class Faculty : Entity
{
    private Faculty()
    {
    }

    private Faculty(Guid id, Name name) : base(id)
    {
        Name = name;
    }

    public static Faculty Create(Guid id, string name)
    {
        return new Faculty(id, new Name(name));
    }

    public Name Name { get; private set; }

    public void Update(Name name)
    {
        Name = name;
    }
}