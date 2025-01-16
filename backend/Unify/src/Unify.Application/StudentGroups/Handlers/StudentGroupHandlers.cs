using Unify.Application.Abstractions.Messaging;
using Unify.Application.StudentGroups.CommandsAndQueries;
using Unify.Domain.Abstractions;
using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;
using Unify.Domain.UniversityCore.Abstractions;
using Unify.Domain.UniversityCore.Errors;
using Unify.Domain.Users;

namespace Unify.Application.StudentGroups.Handlers;

public sealed class GetGroupForUserQueryHandler : IQueryHandler<GetGroupForUserQuery, StudentGroupResponse>
{
    private readonly IStudentGroupRepository _studentGroupRepository;
    private readonly IUserRepository _userRepository;


    public GetGroupForUserQueryHandler(IUserRepository userRepository, IStudentGroupRepository studentGroupRepository)
    {
        _userRepository = userRepository;
        _studentGroupRepository = studentGroupRepository;
    }


    public async Task<Result<StudentGroupResponse>> Handle(GetGroupForUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user == null)
        {
            return Result.Failure<StudentGroupResponse>(StudentGroupErrors.GroupNotFound);
        }

        var group = await _studentGroupRepository.GetByUserAsync(user, cancellationToken);
        if (group == null)
        {
            return Result.Failure<StudentGroupResponse>(StudentGroupErrors.NotEnrolled);
        }

        return Result.Success(StudentGroupResponse.CreateFrom(group));
    }
}

public sealed class GetGroupForSpecializationQueryHandler : IQueryHandler<GetGroupForSpecializationQuery, List<StudentGroupResponse>>
{
    private readonly IStudentGroupRepository _studentGroupRepository;
    private readonly ISpecializationRepository _specializationRepository;

    public GetGroupForSpecializationQueryHandler(IStudentGroupRepository studentGroupRepository, ISpecializationRepository specializationRepository)
    {
        _studentGroupRepository = studentGroupRepository;
        _specializationRepository = specializationRepository;
    }

    public async Task<Result<List<StudentGroupResponse>>> Handle(GetGroupForSpecializationQuery request, CancellationToken cancellationToken)
    {
        var specialization = await _specializationRepository.GetByIdAsync(request.Id, cancellationToken);

        if(specialization == null)
        {
            return Result.Failure<List<StudentGroupResponse>>("SpecializationId.NotFound", "SpecializationId not found");
        }

        var groups = await _studentGroupRepository.GetGroupsBySpecializationAsync(specialization, cancellationToken);
        return Result.Success(groups.Select(StudentGroupResponse.CreateFrom).ToList());
    }
}

public sealed class GetAllGroupsQueryHandler : IQueryHandler<GetAllGroupsQuery, List<StudentGroupResponse>>
{
    private readonly IStudentGroupRepository _studentGroupRepository;

    public GetAllGroupsQueryHandler(IStudentGroupRepository studentGroupRepository)
    {
        _studentGroupRepository = studentGroupRepository;
    }

    public async Task<Result<List<StudentGroupResponse>>> Handle(GetAllGroupsQuery request, CancellationToken cancellationToken)
    {
        var groups = await _studentGroupRepository.GetAllAsync(cancellationToken);
        return Result.Success(groups.Select(StudentGroupResponse.CreateFrom).ToList());
    }
}

public sealed class CreateStudentGroupCommandHandler : ICommandHandler<CreateStudentGroupCommand>
{
    private readonly IStudentGroupRepository _studentGroupRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateStudentGroupCommandHandler(IStudentGroupRepository studentGroupRepository, ISpecializationRepository specializationRepository, IUnitOfWork unitOfWork)
    {
        _studentGroupRepository = studentGroupRepository;
        _specializationRepository = specializationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CreateStudentGroupCommand request, CancellationToken cancellationToken)
    {
        var specialization = await _specializationRepository.GetByIdAsync(request.SpecializationId, cancellationToken);
        if (specialization == null)
        {
            return Result.Failure("SpecializationId.NotFound", "SpecializationId not found");
        }

        var isTermCorrect = Enum.TryParse(request.Term, out Term term);

        if (isTermCorrect == false)
        {
            return Result.Failure(TermErrors.InvalidTerm);
        }

        var group = StudentGroup.Create(new Name(request.Name), specialization, new StudyYear(request.StudyYear),
            new Semester(request.Semester), term, request.MaxGroupSize);

        _studentGroupRepository.Add(group);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

public sealed class CreateStudentGroupsForSpecializationCommandHandler : ICommandHandler<CreateStudentGroupsForSpecializationCommand>
{
    private readonly IStudentGroupRepository _studentGroupRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateStudentGroupsForSpecializationCommandHandler(IStudentGroupRepository studentGroupRepository, ISpecializationRepository specializationRepository, IUnitOfWork unitOfWork)
    {
        _studentGroupRepository = studentGroupRepository;
        _specializationRepository = specializationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CreateStudentGroupsForSpecializationCommand request, CancellationToken cancellationToken)
    {
        var specialization = await _specializationRepository.GetByIdAsync(request.SpecializationId, cancellationToken);
        if (specialization == null)
        {
            return Result.Failure("SpecializationId.NotFound", "SpecializationId not found");
        }

        var isTermCorrect = Enum.TryParse(request.Term, out Term term);

        if (isTermCorrect == false)
        {
            return Result.Failure(TermErrors.InvalidTerm);
        }

        var groups = StudentGroup.CreateMultiple(new Name(request.Name), specialization, new StudyYear(request.StudyYear),
            new Semester(request.Semester), term, request.CombinedSize, request.MaxGroupSize);

        await _studentGroupRepository.AddManyAsync(groups, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

