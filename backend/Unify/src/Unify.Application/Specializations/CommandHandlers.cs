using Unify.Application.Abstractions.Messaging;
using Unify.Domain.Abstractions;
using Unify.Domain.UniversityCore;
using Unify.Domain.UniversityCore.Abstractions;

namespace Unify.Application.Specializations;


internal sealed class AddSpecializationCommandHandler : ICommandHandler<AddSpecializationCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISpecializationRepository _repository;

    public AddSpecializationCommandHandler(IUnitOfWork unitOfWork, ISpecializationRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<Guid>> Handle(AddSpecializationCommand request, CancellationToken cancellationToken)
    {
        var specialization = new Specialization(Guid.NewGuid(), request.Name, request.Description, request.FieldOfStudyId);
        _repository.Add(specialization);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(specialization.Id);
    }
}

internal sealed class UpdateSpecializationCommandHandler : ICommandHandler<UpdateSpecializationCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISpecializationRepository _repository;

    public UpdateSpecializationCommandHandler(IUnitOfWork unitOfWork, ISpecializationRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result> Handle(UpdateSpecializationCommand request, CancellationToken cancellationToken)
    {
        var specialization = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (specialization is null)
        {
            return Result.Failure<Result>("Specialization.NotFound","Specialization not found.");
        }

        specialization.Update(request.Name, request.Description);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

internal sealed class DeleteSpecializationCommandHandler : ICommandHandler<DeleteSpecializationCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISpecializationRepository _repository;

    public DeleteSpecializationCommandHandler(IUnitOfWork unitOfWork, ISpecializationRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result> Handle(DeleteSpecializationCommand request, CancellationToken cancellationToken)
    {
        var specialization = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (specialization is null)
        {
            return Result.Failure<Result>("Specialization.NotFound", "Specialization not found.");
        }

        _repository.Delete(specialization);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}