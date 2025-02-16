using Unify.Application.Abstractions.Authentication;
using Unify.Application.Abstractions.Clock;
using Unify.Application.Abstractions.Messaging;
using Unify.Application.ClassOfferingSessions.CommandsAndQueries;
using Unify.Application.Courses.CourseHandlers;
using Unify.Application.UniversityClasses.ClassOfferings.Commands;
using Unify.Application.Users.GetLoggedInUser;
using Unify.Domain.Abstractions;
using Unify.Domain.Shared;
using Unify.Domain.UniversityClasses;
using Unify.Domain.UniversityClasses.Abstractions;
using Unify.Domain.UniversityCore.Abstractions;
using Unify.Domain.UniversityCore.Errors;
using Unify.Domain.Users;

namespace Unify.Application.ClassOfferings.Handlers;

internal sealed class AddClassOfferingCommandHandler : ICommandHandler<AddClassOfferingCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClassOfferingRepository _repository;
    private readonly ICourseRepository _courseRepository;
    private readonly IUserRepository _userRepository;
    private readonly IStudentGroupRepository _studentGroupRepository;


    public AddClassOfferingCommandHandler(IUnitOfWork unitOfWork, IClassOfferingRepository repository, ICourseRepository courseRepository, IUserRepository userRepository, IUserContext userContext, IStudentGroupRepository studentGroupRepository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _courseRepository = courseRepository;
        _userRepository = userRepository;
        _studentGroupRepository = studentGroupRepository;
    }

    public async Task<Result<Guid>> Handle(AddClassOfferingCommand request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course is null)
        {
            return Result.Failure<Guid>("Course.NotFound", "Course not found.");
        }

        var lecturer = await _userRepository.GetByIdAsync(request.LecturerId, cancellationToken);
        if (lecturer is null || lecturer.Roles.Any(role => role.Id == Role.Lecturer.Id) == false)
        {
            return Result.Failure<Guid>("Lecturer.NotFound", "Lecturer not found.");
        }

        var studentGroup = await _studentGroupRepository.GetByIdAsync(request.StudentGroupId, cancellationToken);

        if (studentGroup == null)
        {
            return Result.Failure<Guid>(StudentGroupErrors.NotFound);
        }

        var classOffering = ClassOffering.Create(new Name(request.Name), course, request.StartDate, request.EndDate, lecturer, studentGroup, request.MaxStudentsCount);
        _repository.Add(classOffering);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(classOffering.Id);
    }
}

internal sealed class GetClassOfferingQueryHandler : IQueryHandler<GetClassOfferingQuery, ClassOfferingResponse>
{
    private readonly IClassOfferingRepository _repository;

    public GetClassOfferingQueryHandler(IClassOfferingRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ClassOfferingResponse>> Handle(GetClassOfferingQuery request, CancellationToken cancellationToken)
    {
        var classOffering = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (classOffering is null)
        {
            return Result.Failure<ClassOfferingResponse>(ClassOfferingErrors.NotFound);
        }

        return ClassOfferingResponse.FromClassOffering(classOffering);
    }
}

internal sealed class UpdateClassOfferingCommandHandler : ICommandHandler<UpdateClassOfferingCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClassOfferingRepository _repository;

    public UpdateClassOfferingCommandHandler(IUnitOfWork unitOfWork, IClassOfferingRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result> Handle(UpdateClassOfferingCommand request, CancellationToken cancellationToken)
    {
        var classOffering = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (classOffering is null)
        {
            return Result.Failure(ClassOfferingErrors.NotFound);
        }

        classOffering.Update(new Name(request.Name), request.StartDate, request.EndDate, request.MaxStudentsCount);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

internal sealed class DeleteClassOfferingCommandHandler : ICommandHandler<DeleteClassOfferingCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClassOfferingRepository _repository;

    public DeleteClassOfferingCommandHandler(IUnitOfWork unitOfWork, IClassOfferingRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result> Handle(DeleteClassOfferingCommand request, CancellationToken cancellationToken)
    {
        var classOffering = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (classOffering is null)
        {
            return Result.Failure(ClassOfferingErrors.NotFound);
        }

        _repository.Delete(classOffering);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

internal sealed class ListClassOfferingsQueryHandler : IQueryHandler<ListClassOfferingsQuery, List<ClassOffering>>
{
    private readonly IClassOfferingRepository _repository;

    public ListClassOfferingsQueryHandler(IClassOfferingRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<ClassOffering>>> Handle(ListClassOfferingsQuery request, CancellationToken cancellationToken)
    {
        var classOfferings = await _repository.GetAllAsync(cancellationToken);
        return Result.Success(classOfferings.ToList());
    }
}


internal sealed class AssignLecturerCommandHandler : ICommandHandler<AssignLecturerCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IClassOfferingRepository _classOfferingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignLecturerCommandHandler(IUserRepository userRepository, IClassOfferingRepository classOfferingRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _classOfferingRepository = classOfferingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AssignLecturerCommand request, CancellationToken cancellationToken)
    {
        var lecturer = await _userRepository.GetByIdAsync(request.LecturerId, cancellationToken);
        if (lecturer == null)
        {
            return Result.Failure(UserErrors.NotFound(request.LecturerId));
        }

        var classOffering = await _classOfferingRepository.GetByIdAsync(request.Id, cancellationToken);
        if (classOffering == null)
        {
            return Result.Failure(ClassOfferingErrors.NotFound);
        }

        classOffering.AssignLecturer(lecturer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

internal sealed class GetClassOfferingsByLecturerQueryHandler: IQueryHandler<GetClassOfferingsByLecturerQuery, List<ClassOfferingResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IClassOfferingRepository _classOfferingRepository;

    public GetClassOfferingsByLecturerQueryHandler(IUserRepository userRepository, IClassOfferingRepository classOfferingRepository)
    {
        _userRepository = userRepository;
        _classOfferingRepository = classOfferingRepository;
    }

    public async Task<Result<List<ClassOfferingResponse>>> Handle(GetClassOfferingsByLecturerQuery request, CancellationToken cancellationToken)
    {
        var lecturer = await _userRepository.GetByIdAsync(request.LecturerId, cancellationToken);
        if (lecturer == null)
        {
            return Result.Failure<List<ClassOfferingResponse>>(UserErrors.NotFound(request.LecturerId));
        }

        var classOfferings = await _classOfferingRepository.GetByLecturerAsync(lecturer, cancellationToken);

        var responses = ClassOfferingResponse.FromClassOfferingList(classOfferings);
        return responses;
    }
}

public sealed class GetStudentsByClassOfferingQueryHandler : IQueryHandler<GetStudentsByClassOfferingQuery, List<UserResponse>>
{
    private readonly IClassOfferingRepository _classOfferingRepository;
    private readonly IUserRepository _userRepository;
    private readonly IStudentGroupRepository _studentGroupRepository;

    public GetStudentsByClassOfferingQueryHandler(IClassOfferingRepository classOfferingRepository, IUserRepository userRepository, IStudentGroupRepository studentGroupRepository)
    {
        _classOfferingRepository = classOfferingRepository;
        _userRepository = userRepository;
        _studentGroupRepository = studentGroupRepository;
    }

    public async Task<Result<List<UserResponse>>> Handle(GetStudentsByClassOfferingQuery request, CancellationToken cancellationToken)
    {
        var classOffering = await _classOfferingRepository.GetByIdAsync(request.Id, cancellationToken);
        if (classOffering == null)
        {
            return ClassOfferingErrors.NotFound;
        }

        var studentGroup = await _studentGroupRepository.GetByIdAsync(classOffering.StudentGroupId, cancellationToken);

        if (studentGroup == null)
        {
            return new List<UserResponse>();
        }


        var students = await _userRepository.GetManyByIdAsync(studentGroup.Members.Select(member => member.Id).ToList(),
            cancellationToken);

        var responses = UserResponse.FromUsers(students);
        return responses;
    }
}

public sealed class GetClassOfferingsByStudentQueryHandler : IQueryHandler<GetClassOfferingsByStudentQuery, List<ClassOfferingResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IClassEnrollmentRepository _classEnrollmentRepository;
    private readonly IClassOfferingRepository _classOfferingRepository;

    public GetClassOfferingsByStudentQueryHandler(IUserRepository userRepository, IClassEnrollmentRepository classEnrollmentRepository, IClassOfferingRepository classOfferingRepository)
    {
        _userRepository = userRepository;
        _classEnrollmentRepository = classEnrollmentRepository;
        _classOfferingRepository = classOfferingRepository;
    }

    public async Task<Result<List<ClassOfferingResponse>>> Handle(GetClassOfferingsByStudentQuery request, CancellationToken cancellationToken)
    {
        var student = await _userRepository.GetByIdAsync(request.StudentId, cancellationToken);
        if(student == null)
        {
            return Result.Failure<List<ClassOfferingResponse>>(UserErrors.NotFound(request.StudentId));
        }

        var classEnrollments = await _classEnrollmentRepository.GetByStudentIdAsync(student.Id, cancellationToken);

        var classOfferings = classEnrollments.Select(e => _classOfferingRepository.GetByIdAsync(e.ClassOfferingId, cancellationToken)).ToList();

        var responses = await Task.WhenAll(classOfferings);
        if(responses == null || responses.Length == 0)
        {
            return new List<ClassOfferingResponse>();
        }
        return ClassOfferingResponse.FromClassOfferingList(responses.Where(r => r != null).ToList()!);
    }
}