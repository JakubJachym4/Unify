using Unify.Domain.Abstractions;

namespace Unify.Application.Abstractions.Validations;

public interface IValidation
{
    IValidation SetNext(IValidation next);

    Result<T> Validate<T>(T input);
}