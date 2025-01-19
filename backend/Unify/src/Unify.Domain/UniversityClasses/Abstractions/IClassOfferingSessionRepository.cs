using Unify.Domain.UniversityClasses;

namespace Unify.Domain.UniversityCore.Abstractions;

public interface IClassOfferingSessionRepository
{
    Task<List<ClassOfferingSession>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ClassOfferingSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    void Add(ClassOfferingSession entity);
    void Delete(ClassOfferingSession entity);
}