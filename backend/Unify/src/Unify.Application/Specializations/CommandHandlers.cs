using Unify.Application.Abstractions.Messaging;
using Unify.Domain.Abstractions;
using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;
using Unify.Domain.UniversityCore.Abstractions;
using Unify.Domain.UniversityCore.Errors;
using Unify.Domain.Users;

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
        var specialization = new Specialization(Guid.NewGuid(), new Name(request.Name), new Description(request.Description), request.FieldOfStudyId);
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
            return Result.Failure<Result>("SpecializationId.NotFound","SpecializationId not found.");
        }

        specialization.Update(new Name(request.Name), new Description(request.Description));
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
            return Result.Failure<Result>("SpecializationId.NotFound", "SpecializationId not found.");
        }

        _repository.Delete(specialization);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public class AssignStudentToSpecializationCommandHandler : ICommandHandler<AssignStudentToSpecializationCommand>
{
    private ISpecializationRepository _specializationRepository;
    private IUserRepository _userRepository;
    private IUnitOfWork _unitOfWork;
    private IStudentGroupRepository _studentGroupRepository;

    public AssignStudentToSpecializationCommandHandler(IUserRepository userRepository, ISpecializationRepository specializationRepository, IUnitOfWork unitOfWork, IStudentGroupRepository studentGroupRepository)
    {
        _userRepository = userRepository;
        _specializationRepository = specializationRepository;
        _unitOfWork = unitOfWork;
        _studentGroupRepository = studentGroupRepository;
    }

    public async Task<Result> Handle(AssignStudentToSpecializationCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.StudentId, cancellationToken);
        if (user == null)
        {
            return Result.Failure(UserErrors.NotFound(request.StudentId));
        }
        var specialization = await _specializationRepository.GetByIdAsync(request.SpecializationId, cancellationToken);
        if (specialization == null)
        {
            return Result.Failure<Result>("SpecializationId.NotFound", "SpecializationId not found.");
        }

        var group = await _studentGroupRepository.GetByUserAsync(user, cancellationToken);
        if (group != null)
        {
            return Result.Failure(StudentGroupErrors.UserPresentInSpecializationGroup);
        }

        specialization.AssignStudent(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public class UnassignStudentFromSpecializationCommandHandler : ICommandHandler<UnassignStudentFromSpecializationCommand>
{
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStudentGroupRepository _studentGroupRepository;

    public UnassignStudentFromSpecializationCommandHandler(IUserRepository userRepository, ISpecializationRepository specializationRepository, IUnitOfWork unitOfWork, IStudentGroupRepository studentGroupRepository)
    {
        _userRepository = userRepository;
        _specializationRepository = specializationRepository;
        _unitOfWork = unitOfWork;
        _studentGroupRepository = studentGroupRepository;
    }

    public async Task<Result> Handle(UnassignStudentFromSpecializationCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.StudentId, cancellationToken);
        if (user == null)
        {
            return Result.Failure(UserErrors.NotFound(request.StudentId));
        }
        var specialization = await _specializationRepository.GetByIdAsync(request.SpecializationId, cancellationToken);
        if (specialization == null)
        {
            return Result.Failure<Result>("SpecializationId.NotFound", "SpecializationId not found.");
        }
        var group = await _studentGroupRepository.GetByUserAsync(user, cancellationToken);
        if (group != null)
        {
            return Result.Failure(StudentGroupErrors.UserPresentInSpecializationGroup);
        }

        specialization.UnassignStudent(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}