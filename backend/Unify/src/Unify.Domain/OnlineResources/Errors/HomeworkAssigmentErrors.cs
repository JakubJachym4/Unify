using Unify.Domain.Abstractions;

namespace Unify.Domain.OnlineResources.Errors;

public static class HomeworkAssigmentErrors
{
    public static Error NotFound =>
        new("HomeworkAssignment.NotFound", "Homework assignment not found.");
}