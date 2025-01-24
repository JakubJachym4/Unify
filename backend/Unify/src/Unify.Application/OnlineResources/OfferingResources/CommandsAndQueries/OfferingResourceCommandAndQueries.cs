using Microsoft.AspNetCore.Http;
using Unify.Application.Abstractions.Messaging;

namespace Unify.Application.OnlineResources.OfferingResources.CommandsAndQueries;

public record CreateOfferingResourceCommand(Guid ClassOfferingId, string Title, string Description, List<IFormFile>? Attachments) : ICommand<Guid>;

public record UpdateOfferingResourceCommand(Guid Id, string Title, string Description, List<IFormFile>? Attachments) : ICommand;

public record DeleteOfferingResourceCommand(Guid Id) : ICommand;

public record GetOfferingResourcesQuery(Guid Id) : IQuery<List<OfferingResourceResponse>>;

public record GetOfferingResourceQuery(Guid Id) : IQuery<OfferingResourceResponse>;

