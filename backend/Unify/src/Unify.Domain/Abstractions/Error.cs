namespace Unify.Domain.Abstractions;

public record Error(string Code, string Details)
{
    public static Error None = new(string.Empty, string.Empty);

    public static Error NullValue = new("Error.NullValue", "Null value was provided");

    public static Error Create(string code, string details) => new(code, details);
    public static Error Create(string code, string details, params object[] args) =>
        new(code, string.Format(details, args));
}