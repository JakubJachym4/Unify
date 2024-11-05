namespace Unify.Application.Abstractions.Clock;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateOnly UtcNowDateOnly { get; }
}