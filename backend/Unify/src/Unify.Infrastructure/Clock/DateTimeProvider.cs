using Unify.Application.Abstractions.Clock;

namespace Unify.Infrastructure.Clock;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateOnly UtcNowDateOnly => DateOnly.FromDateTime(DateTime.UtcNow);
}