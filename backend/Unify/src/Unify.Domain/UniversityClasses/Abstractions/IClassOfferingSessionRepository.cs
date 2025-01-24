using Unify.Domain.UniversityClasses;

namespace Unify.Domain.UniversityCore.Abstractions;

public interface IClassOfferingSessionRepository
{
    Task<List<ClassOfferingSession>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ClassOfferingSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    void Add(ClassOfferingSession entity);
    void Delete(ClassOfferingSession entity);
    Task<List<ClassOfferingSession>> GetByClassOfferingIdAsync(Guid classOfferingId, CancellationToken cancellationToken);
    Task<List<ClassOfferingSession>> GetByLecturerIdAsync(Guid lecturerId, CancellationToken cancellationToken);
}