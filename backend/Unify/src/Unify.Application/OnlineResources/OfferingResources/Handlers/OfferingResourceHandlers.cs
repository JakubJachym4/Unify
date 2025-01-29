using Unify.Application.Abstractions.Files;
using Unify.Application.Abstractions.Messaging;
using Unify.Application.OnlineResources.OfferingResources.CommandsAndQueries;
using Unify.Domain.Abstractions;
using Unify.Domain.OnlineResources;
using Unify.Domain.OnlineResources.Abstraction;
using Unify.Domain.Shared;
using Unify.Domain.UniversityClasses;
using Unify.Domain.UniversityClasses.Abstractions;

namespace Unify.Application.OnlineResources.OfferingResources.Handlers;

public sealed class CreateOfferingResourceCommandHandler : ICommandHandler<CreateOfferingResourceCommand, Guid>
{
    private readonly IClassOfferingRepository _classOfferingRepository;
    private readonly IOfferingResourceRepository _offeringResourceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileConversionService _fileConversionService;

    public CreateOfferingResourceCommandHandler(IClassOfferingRepository classOfferingRepository, IOfferingResourceRepository offeringResourceRepository, IUnitOfWork unitOfWork, IFileConversionService fileConversionService)
    {
        _classOfferingRepository = classOfferingRepository;
        _offeringResourceRepository = offeringResourceRepository;
        _unitOfWork = unitOfWork;
        _fileConversionService = fileConversionService;
    }

    public async Task<Result<Guid>> Handle(CreateOfferingResourceCommand request, CancellationToken cancellationToken)
    {
        var classOffering = await _classOfferingRepository.GetByIdAsync(request.ClassOfferingId, cancellationToken);
        if (classOffering is null)
        {
            return Result.Failure<Guid>("ClassOffering.NotFound", "Class offering not found.");
        }

        var offeringResource = new OfferingResource(classOffering, new Title(request.Title), new Description(request.Description));

        if (request.Attachments != null)
        {
            var attachments = await _fileConversionService.ConvertToAttachments(request.Attachments);

            foreach (var attachment in attachments)
            {
                if (attachment.IsFailure)
                {
                    return Result.Failure<Guid>(attachment.Error);
                }
                offeringResource.AddFile(attachment.Value);
            }
        }

        _offeringResourceRepository.Add(offeringResource);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(offeringResource.Id);
    }
}

public sealed class UpdateOfferingResourceCommandHandler : ICommandHandler<UpdateOfferingResourceCommand>
{
    private readonly IOfferingResourceRepository _offeringResourceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileConversionService _fileConversionService;

    public UpdateOfferingResourceCommandHandler(IOfferingResourceRepository offeringResourceRepository, IUnitOfWork unitOfWork, IFileConversionService fileConversionService)
    {
        _offeringResourceRepository = offeringResourceRepository;
        _unitOfWork = unitOfWork;
        _fileConversionService = fileConversionService;
    }

    public async Task<Result> Handle(UpdateOfferingResourceCommand request, CancellationToken cancellationToken)
    {
        var offeringResource = await _offeringResourceRepository.GetByIdAsyncIncludeAttachments(request.Id, cancellationToken);
        if (offeringResource is null)
        {
            return Result.Failure("OfferingResource.NotFound", "Offering resource not found.");
        }

        if (request.Attachments != null)
        {
            var attachments = await _fileConversionService.ConvertToAttachments(request.Attachments);
            offeringResource.ClearFiles();

            foreach (var attachment in attachments)
            {
                if (attachment.IsFailure)
                {
                    return Result.Failure<Guid>(attachment.Error);
                }
                offeringResource.AddFile(attachment.Value);
            }
        }

        offeringResource.Update(new Title(request.Title), new Description(request.Description));
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public sealed class DeleteOfferingResourceCommandHandler : ICommandHandler<DeleteOfferingResourceCommand>
{
    private readonly IOfferingResourceRepository _offeringResourceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteOfferingResourceCommandHandler(IOfferingResourceRepository offeringResourceRepository, IUnitOfWork unitOfWork)
    {
        _offeringResourceRepository = offeringResourceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteOfferingResourceCommand request, CancellationToken cancellationToken)
    {
        var offeringResource = await _offeringResourceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (offeringResource is null)
        {
            return Result.Failure("OfferingResource.NotFound", "Offering resource not found.");
        }

        _offeringResourceRepository.Delete(offeringResource);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public sealed class GetOfferingResourceQueryHandler : IQueryHandler<GetOfferingResourceQuery, OfferingResourceResponse>
{
    private readonly IOfferingResourceRepository _offeringResourceRepository;

    public GetOfferingResourceQueryHandler(IOfferingResourceRepository offeringResourceRepository)
    {
        _offeringResourceRepository = offeringResourceRepository;
    }

    public async Task<Result<OfferingResourceResponse>> Handle(GetOfferingResourceQuery request, CancellationToken cancellationToken)
    {
        var offeringResource = await _offeringResourceRepository.GetByIdAsyncIncludeAttachments(request.Id, cancellationToken);
        if (offeringResource == null)
        {
            return Result.Failure<OfferingResourceResponse>("OfferingResource.NotFound", "Offering resource not found.");
        }

        return Result.Success(OfferingResourceResponse.CreateFromOfferingResource(offeringResource));
    }
}

public sealed class GetOfferingResourcesQueryHandler : IQueryHandler<GetOfferingResourcesQuery, List<OfferingResourceResponse>>
{
    private readonly IOfferingResourceRepository _offeringResourceRepository;
    private readonly IClassOfferingRepository _classOfferingRepository;

    public GetOfferingResourcesQueryHandler(IOfferingResourceRepository offeringResourceRepository, IClassOfferingRepository classOfferingRepository)
    {
        _offeringResourceRepository = offeringResourceRepository;
        _classOfferingRepository = classOfferingRepository;
    }

    public async Task<Result<List<OfferingResourceResponse>>> Handle(GetOfferingResourcesQuery request, CancellationToken cancellationToken)
    {
        var classOffering = await _classOfferingRepository.GetByIdAsync(request.Id, cancellationToken);

        if(classOffering == null)
        {
            return Result.Failure<List<OfferingResourceResponse>>(ClassOfferingErrors.NotFound);
        }

        var offeringResources = await _offeringResourceRepository.GetByClassOfferingAsyncIncludeAttachments(classOffering, cancellationToken);

        return Result.Success(offeringResources.Select(OfferingResourceResponse.CreateFromOfferingResource).ToList());
    }
}