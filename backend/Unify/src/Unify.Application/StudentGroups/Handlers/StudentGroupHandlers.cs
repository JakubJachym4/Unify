using Unify.Application.Abstractions.Authentication;
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
            return Result.Failure<StudentGroupResponse>(StudentGroupErrors.NotFound);
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

public sealed class JoinGroupCommandHandler : ICommandHandler<JoinGroupCommand>
{
    private readonly IStudentGroupRepository _studentGroupRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;

    public JoinGroupCommandHandler(IStudentGroupRepository studentGroupRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _studentGroupRepository = studentGroupRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result> Handle(JoinGroupCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(_userContext.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Failure(UserErrors.NotFound(_userContext.UserId));
        }

        var group = await _studentGroupRepository.GetByIdAsync(request.Id, cancellationToken);

        if (group == null)
        {
            return Result.Failure(StudentGroupErrors.NotFound);
        }

        group.Join(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public sealed class MoveUserToGroupCommandHandler : ICommandHandler<MoveUserToGroupCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IStudentGroupRepository _studentGroupRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MoveUserToGroupCommandHandler(IUserRepository userRepository, IStudentGroupRepository studentGroupRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _studentGroupRepository = studentGroupRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(MoveUserToGroupCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Failure(UserErrors.NotFound(request.UserId));
        }
        var currentGroup = await _studentGroupRepository.GetByUserAsync(user, cancellationToken);

        if(request.GroupId == null)
        {
            if (currentGroup == null)
                return Result.Success();

            var result = currentGroup.Leave(user);
            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }
        }
        else
        {
            var wantedGroup = await _studentGroupRepository.GetByIdAsync(request.GroupId.Value, cancellationToken);
            if (wantedGroup == null)
            {
                return Result.Failure(StudentGroupErrors.NotFound);
            }

            if (currentGroup != null)
            {
                var result = currentGroup.Leave(user);
                if (result.IsFailure)
                {
                    return Result.Failure(result.Error);
                }
            }
            var joinResult = wantedGroup.Join(user);
            if (joinResult.IsFailure)
            {
                return Result.Failure(joinResult.Error);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public sealed class UpdateStudentGroupCommandHandler : ICommandHandler<UpdateStudentGroupCommand>
{
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IStudentGroupRepository _studentGroupRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStudentGroupCommandHandler(IStudentGroupRepository studentGroupRepository, IUnitOfWork unitOfWork, ISpecializationRepository specializationRepository)
    {
        _studentGroupRepository = studentGroupRepository;
        _unitOfWork = unitOfWork;
        _specializationRepository = specializationRepository;
    }

    public async Task<Result> Handle(UpdateStudentGroupCommand request, CancellationToken cancellationToken)
    {

        var group = await _studentGroupRepository.GetByIdAsync(request.Id, cancellationToken);

        if (group == null)
        {
            return Result.Failure(StudentGroupErrors.NotFound);
        }
        var isCorrectTerm = Enum.TryParse(request.Term, out Term term);
        if (isCorrectTerm == false)
        {
            return Result.Failure(TermErrors.InvalidTerm);
        }

        group.Update(new Name(request.Name), new StudyYear(request.StudyYear), new Semester(request.Semester), term, request.MaxGroupSize);

        if (request.SpecializationId != null)
        {
            var specialization = await _specializationRepository.GetByIdAsync(request.SpecializationId.Value, cancellationToken);
            if (specialization == null)
            {
                return Result.Failure("SpecializationId.NotFound", "Specialization not found");
            }
            group.ChangeSpecialization(specialization);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public sealed class DeleteStudentGroupCommandHandler : ICommandHandler<DeleteStudentGroupCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStudentGroupRepository _studentGroupRepository;

    public DeleteStudentGroupCommandHandler(IUnitOfWork unitOfWork, IStudentGroupRepository studentGroupRepository)
    {
        _unitOfWork = unitOfWork;
        _studentGroupRepository = studentGroupRepository;
    }

    public async Task<Result> Handle(DeleteStudentGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _studentGroupRepository.GetByIdAsync(request.Id, cancellationToken);
        if(group == null)
        {
            return Result.Failure(StudentGroupErrors.NotFound);
        }

        if(group.Members.Any())
        {
            return Result.Failure(StudentGroupErrors.NotEmpty);
        }

        _studentGroupRepository.Delete(group);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public sealed class AutoAssignStudentsToGroupsCommandHandler : ICommandHandler<AutoAssignStudentsToGroupsCommand>
{
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IStudentGroupRepository _studentGroupRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AutoAssignStudentsToGroupsCommandHandler(ISpecializationRepository specializationRepository, IStudentGroupRepository studentGroupRepository, IUnitOfWork unitOfWork)
    {
        _specializationRepository = specializationRepository;
        _studentGroupRepository = studentGroupRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AutoAssignStudentsToGroupsCommand request, CancellationToken cancellationToken)
    {
        var specialization = await _specializationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (specialization == null)
        {
            return Result.Failure("SpecializationId.NotFound", "Specialization not found");
        }

        var groups = await _studentGroupRepository.GetGroupsBySpecializationAsync(specialization, cancellationToken);

        if (groups.Count == 0)
        {
            return Result.Failure(StudentGroupErrors.NotFound);
        }

        groups = groups.OrderBy(g => g.Name.Value).ToList();

        var students = await _specializationRepository.GetStudentsAsync(specialization, cancellationToken);
        var maxGroupSize = groups[0].MaxGroupSize;
        int groupIndex = 0;

        for (int i = 0; i < students.Count; i++)
        {
            var student = students[i];
            var group = groups[groupIndex];
            if(groups.All(studentGroup => studentGroup.Members.All(s => s.Id != student.Id)))
            {
                var result = group.Join(student);
                if (result.IsFailure)
                {
                    return Result.Failure(result.Error);
                }
            }
            else
            {
                return Result.Failure(StudentGroupErrors.CannotAssignStudent);
            }

            if (i > 0 && i % maxGroupSize == 0)
                groupIndex++;
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

