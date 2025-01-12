using MediatR;
using Unify.Application.Abstractions.Messaging;
using Unify.Domain.Abstractions;
using Unify.Domain.Shared;

namespace Unify.Application.FieldsOfStudy;

public record AddFieldOfStudyCommand(string Name, string Description, Guid FacultyId) : ICommand<Guid>;
public record UpdateFieldOfStudyCommand(Guid Id, string Name, string Description) : ICommand;
public record DeleteFieldOfStudyCommand(Guid Id) : ICommand;