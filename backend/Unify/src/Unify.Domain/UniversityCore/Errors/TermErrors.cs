using Unify.Domain.Abstractions;

namespace Unify.Domain.UniversityCore.Errors;

public static class TermErrors
{
    public static Error InvalidTerm =>
        Error.Create("Term.Invalid",
            "Specified term is invalid.");
}