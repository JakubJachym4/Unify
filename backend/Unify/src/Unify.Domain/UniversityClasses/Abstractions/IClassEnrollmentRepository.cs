namespace Unify.Domain.UniversityClasses.Abstractions;

public interface IClassEnrollmentRepository
{
    Task<List<ClassEnrollment>> GetByClassOfferingIdAsync(Guid classOfferingId, CancellationToken cancellationToken = default);
    Task<List<ClassEnrollment>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);



    Task<ClassEnrollment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ClassEnrollment?> GetByClassOfferingAndStudentAsync(Guid classOfferingId, Guid studentId, CancellationToken cancellationToken);

    void Add(ClassEnrollment entity);

    void Delete(ClassEnrollment entity);
}