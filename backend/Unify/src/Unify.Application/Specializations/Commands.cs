using MediatR;
using Unify.Application.Abstractions.Messaging;
using Unify.Domain.Abstractions;
using Unify.Domain.Shared;

namespace Unify.Application.Specializations;


public record AddSpecializationCommand(string Name, string Description, Guid FieldOfStudyId) : ICommand<Guid>;
public record UpdateSpecializationCommand(Guid Id, string Name, string Description) : ICommand;
public record DeleteSpecializationCommand(Guid Id) : ICommand;

public record AssignStudentToSpecializationCommand(Guid StudentId, Guid SpecializationId) : ICommand;

public record UnassignStudentFromSpecializationCommand(Guid StudentId, Guid SpecializationId) : ICommand;