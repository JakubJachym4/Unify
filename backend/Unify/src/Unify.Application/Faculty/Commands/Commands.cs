using Unify.Application.Abstractions.Messaging;

namespace Unify.Application.Faculty.Commands;

public record AddFacultyCommand(string Name) : ICommand<Guid>;
public record UpdateFacultyCommand(Guid Id, string Name) : ICommand;
public record DeleteFacultyCommand(Guid Id) : ICommand;
public record ListFacultyQuery() : IQuery<List<Domain.UniversityCore.Faculty>>;