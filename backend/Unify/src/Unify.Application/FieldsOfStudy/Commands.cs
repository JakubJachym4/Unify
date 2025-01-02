using MediatR;
using Unify.Application.Abstractions.Messaging;
using Unify.Domain.Abstractions;
using Unify.Domain.Shared;

namespace Unify.Application.FieldsOfStudy;

public record AddFieldOfStudyCommand(Name Name, Description Description, Guid FacultyId) : ICommand<Guid>;
public record UpdateFieldOfStudyCommand(Guid Id, Name Name, Description Description) : ICommand;
public record DeleteFieldOfStudyCommand(Guid Id) : IRequest<Result>, ICommand;