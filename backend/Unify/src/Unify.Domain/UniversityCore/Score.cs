using System.ComponentModel.DataAnnotations.Schema;

namespace Unify.Domain.UniversityCore;

[NotMapped]
public sealed record Score
{
    public decimal Value { get; init; }

    public static implicit operator Score(decimal value) => new Score { Value = value };
};