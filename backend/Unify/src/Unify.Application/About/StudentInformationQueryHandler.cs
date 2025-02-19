using Unify.Application.Abstractions.Authentication;
using Unify.Application.Abstractions.Messaging;
using Unify.Domain.Abstractions;
using Unify.Domain.UniversityCore.Abstractions;
using Unify.Domain.UniversityCore.Errors;
using Unify.Domain.Users;

namespace Unify.Application.About;

internal sealed class StudentInformationQueryHandler : IQueryHandler<GetStudentInformationQuery, StudentInformationResponse>
{
    private readonly IUserContext _userContext;
    private readonly IUserRepository _userRepository;
    private readonly IStudentGroupRepository _studentGroupRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IFieldOfStudyRepository _fieldOfStudyRepository;
    private readonly IFacultyRepository _facultyRepository;

    public StudentInformationQueryHandler(IStudentGroupRepository studentGroupRepository, IUserContext userContext, IUserRepository userRepository, ISpecializationRepository specializationRepository, IFieldOfStudyRepository fieldOfStudyRepository, IFacultyRepository facultyRepository)
    {
        _studentGroupRepository = studentGroupRepository;
        _userContext = userContext;
        _userRepository = userRepository;
        _specializationRepository = specializationRepository;
        _fieldOfStudyRepository = fieldOfStudyRepository;
        _facultyRepository = facultyRepository;
    }

    public async Task<Result<StudentInformationResponse>> Handle(GetStudentInformationQuery request, CancellationToken cancellationToken)
    {
        var student = await _userRepository.GetByIdAsync(_userContext.UserId, cancellationToken);
        if (student == null)
        {
            return UserErrors.NotFound(_userContext.UserId);
        }

        var studentGroup = await _studentGroupRepository.GetByUserAsync(student, cancellationToken);
        if (studentGroup == null)
        {
            return StudentGroupErrors.NotEnrolled;
        }

        var specialization = await _specializationRepository.GetByIdAsync(studentGroup.SpecializationId, cancellationToken);
        if (specialization == null)
        {
            return new Error("Specialization.NotFound", "Specialization not found");
        }

        var fieldOfStudy = await _fieldOfStudyRepository.GetByIdAsync(specialization.FieldOfStudyId, cancellationToken);
        if (fieldOfStudy == null)
        {
            return new Error("FieldOfStudy.NotFound", "Field of study not found");
        }

        var faculty = await _facultyRepository.GetByIdAsync(fieldOfStudy.FacultyId, cancellationToken);
        if (faculty == null)
        {
            return new Error("Faculty.NotFound", "Faculty not found");
        }


        var response = new StudentInformationResponse
            (student.Id,
            student.FirstName.Value,
            student.LastName.Value,
            student.Email.Value,
            studentGroup.Name.Value,
            fieldOfStudy.Name.Value,
            specialization.Name.Value,
            studentGroup.Semester.Value,
            studentGroup.Term.ToString(),
            studentGroup.StudyYear.ToString(),
            faculty.Name.Value
            );

        return response;
    }
}