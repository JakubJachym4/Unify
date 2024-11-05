using Unify.Domain.Abstractions;

namespace Unify.Application.Abstractions.Validations;

public abstract class AbstractValidator : IValidation
{
    private IValidation? _next;

    public IValidation SetNext(IValidation next)
    {
        _next = next;

        return next;
    }

    public virtual Result<T> Validate<T>(T input)
    {
        if (_next != null)
        {
            return _next.Validate(input);
        }

        return Result.Success(input);
    }
}