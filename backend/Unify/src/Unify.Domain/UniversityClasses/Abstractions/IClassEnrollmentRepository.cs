namespace Unify.Domain.UniversityClasses.Abstractions;

public interface IClassEnrollmentRepository
{
    Task<List<ClassEnrollment>> GetByClassOfferingIdAsync(Guid classOfferingId, CancellationToken cancellationToken = default);
    Task<List<ClassEnrollment>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);

    void Add(ClassEnrollment entity);

    void Delete(ClassEnrollment entity);
}