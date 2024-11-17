namespace Unify.Domain.FieldsOfStudy;

public sealed record Score
{
    public decimal Value { get; init; }

    public static implicit operator Score(decimal value) => new Score { Value = value };
};