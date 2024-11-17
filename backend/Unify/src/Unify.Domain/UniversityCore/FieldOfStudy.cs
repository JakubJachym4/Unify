using Unify.Domain.Abstractions;
using Unify.Domain.Shared;

namespace Unify.Domain.UniversityCore;

public sealed class FieldOfStudy : Entity
{
    public Name Name { get; private set; }
    public Description Description { get; private set; }
    public Faculty Faculty { get; private set; }

    private readonly List<Specialization> _specializations = new();
    public IReadOnlyCollection<Specialization> Specializations => _specializations;
}