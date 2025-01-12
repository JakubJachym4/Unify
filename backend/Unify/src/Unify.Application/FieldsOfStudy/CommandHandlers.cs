using Unify.Application.Abstractions.Messaging;
using Unify.Domain.Abstractions;
using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;
using Unify.Domain.UniversityCore.Abstractions;

namespace Unify.Application.FieldsOfStudy;

internal sealed class AddFieldOfStudyCommandHandler : ICommandHandler<AddFieldOfStudyCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFieldOfStudyRepository _repository;

    public AddFieldOfStudyCommandHandler(IUnitOfWork unitOfWork, IFieldOfStudyRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<Guid>> Handle(AddFieldOfStudyCommand request, CancellationToken cancellationToken)
    {
        var foundFieldOfStudy = await _repository.GetByNameAsync(request.Name, cancellationToken);

        if (foundFieldOfStudy is not null)
        {
            return Result.Failure<Guid>("FieldOfStudy.AlreadyExists", "Field of Study already exists.");
        }

        var fieldOfStudy = new FieldOfStudy(Guid.NewGuid(), new Name(request.Name), new Description(request.Description), request.FacultyId);

        _repository.Add(fieldOfStudy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(fieldOfStudy.Id);
    }
}

internal sealed class UpdateFieldOfStudyCommandHandler : ICommandHandler<UpdateFieldOfStudyCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFieldOfStudyRepository _repository;

    public UpdateFieldOfStudyCommandHandler(IUnitOfWork unitOfWork, IFieldOfStudyRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result> Handle(UpdateFieldOfStudyCommand request, CancellationToken cancellationToken)
    {
        var fieldOfStudy = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (fieldOfStudy is null)
        {
            return Result.Failure(Error.NullValue);
        }

        fieldOfStudy.Update(new Name(request.Name), new Description(request.Description));
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

internal sealed class DeleteFieldOfStudyCommandHandler : ICommandHandler<DeleteFieldOfStudyCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFieldOfStudyRepository _repository;

    public DeleteFieldOfStudyCommandHandler(IUnitOfWork unitOfWork, IFieldOfStudyRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result> Handle(DeleteFieldOfStudyCommand request, CancellationToken cancellationToken)
    {
        var fieldOfStudy = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (fieldOfStudy is null)
        {
            return Result.Failure(Error.NullValue);
        }

        _repository.Delete(fieldOfStudy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}