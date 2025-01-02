using MediatR;
using Unify.Application.Abstractions.Messaging;
using Unify.Domain.Abstractions;
using Unify.Domain.Shared;

namespace Unify.Application.Specializations;


public record AddSpecializationCommand(Name Name, Description Description, Guid FieldOfStudyId) : ICommand<Guid>;
public record UpdateSpecializationCommand(Guid Id, Name Name, Description Description) : ICommand;
public record DeleteSpecializationCommand(Guid Id) : ICommand;