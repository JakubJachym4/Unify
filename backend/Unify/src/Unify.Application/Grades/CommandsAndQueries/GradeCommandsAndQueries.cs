using Unify.Application.Abstractions.Messaging;
using Unify.Application.ClassEnrollment;

namespace Unify.Application.Grades.CommandsAndQueries;

public record CreateMarkCommand(Guid GradeId, string Title, decimal Score, decimal MaxScore) : ICommand;

public record AwardGradeCommand(Guid Id, bool Awarded) : ICommand;
public record GetGradeQuery(Guid Id) : IQuery<GradeResponse>;