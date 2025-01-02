using Unify.Domain.Shared;

namespace Unify.Domain.UniversityCore.Abstractions;

public interface IFieldOfStudyRepository
{
    Task<FieldOfStudy?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<FieldOfStudy?> GetByIdAsyncNoTracking(Guid id, CancellationToken cancellationToken);
    Task<FieldOfStudy?> GetByNameAsync(Name name, CancellationToken cancellationToken);
    Task<List<FieldOfStudy>> GetAllAsync(CancellationToken cancellationToken);
    void Add(FieldOfStudy entity);
    void Delete(FieldOfStudy entity);


}