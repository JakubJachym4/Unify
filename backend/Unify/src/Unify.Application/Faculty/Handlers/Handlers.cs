using MediatR;
using Unify.Application.Abstractions.Messaging;
using Unify.Application.Faculty.Commands;
using Unify.Application.Specializations;
using Unify.Domain.Abstractions;
using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;
using Unify.Domain.UniversityCore.Abstractions;


namespace Unify.Application.Faculty.Handlers;



internal sealed class AddFacultyCommandHandler : ICommandHandler<AddFacultyCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFacultyRepository _repository;

    public AddFacultyCommandHandler(IUnitOfWork unitOfWork, IFacultyRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<Guid>> Handle(AddFacultyCommand request, CancellationToken cancellationToken)
    {
        var foundFaculty = _repository.GetByName(request.Name, cancellationToken);

        if (foundFaculty is not null)
        {
            return Result.Failure<Guid>("FieldOfStudy.AlreadyExists", "Field of Study already exists.");
        }

        var faculty = Domain.UniversityCore.Faculty.Create(Guid.NewGuid(), request.Name);

        _repository.Add(faculty);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(faculty.Id);
    }
}

internal sealed class UpdateFacultyCommandHandler : ICommandHandler<UpdateFacultyCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFacultyRepository _repository;

    public UpdateFacultyCommandHandler(IUnitOfWork unitOfWork, IFacultyRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result> Handle(UpdateFacultyCommand request, CancellationToken cancellationToken)
    {
        var faculty = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (faculty is null)
        {
            return Result.Failure(Error.NullValue);
        }

        faculty.Update(new Name(request.Name));
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

internal sealed class DeleteFacultyCommandHandler : ICommandHandler<DeleteFacultyCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFacultyRepository _repository;

    public DeleteFacultyCommandHandler(IUnitOfWork unitOfWork, IFacultyRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result> Handle(DeleteFacultyCommand request, CancellationToken cancellationToken)
    {
        var faculty = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (faculty is null)
        {
            return Result.Failure(Error.NullValue);
        }

        _repository.Delete(faculty);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

internal sealed class ListFacultiesQueryHandler : IRequestHandler<ListFacultyQuery, Result<List<FacultyResult>>>
{
    private readonly IFacultyRepository _repository;

    public ListFacultiesQueryHandler(IFacultyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<FacultyResult>>> Handle(ListFacultyQuery request, CancellationToken cancellationToken)
    {
        var faculties = await _repository.GetAllAsync(cancellationToken);
        return Result.Success(faculties.Select(f => new FacultyResult(f.Id, f.Name.Value)).ToList());
    }
}

public record FacultyResult(Guid Id, string Name);