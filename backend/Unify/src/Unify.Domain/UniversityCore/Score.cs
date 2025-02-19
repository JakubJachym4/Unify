using System.ComponentModel.DataAnnotations.Schema;

namespace Unify.Domain.UniversityCore;

[NotMapped]
public sealed record Score(decimal Value)
{
    public static implicit operator Score(decimal value) => new(value);
};