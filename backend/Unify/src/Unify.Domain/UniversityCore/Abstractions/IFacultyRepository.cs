namespace Unify.Domain.UniversityCore.Abstractions;

public interface IFacultyRepository
{
    Task<Faculty?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Faculty?> GetByIdAsyncNoTracking(Guid id, CancellationToken cancellationToken);
    Task<Faculty?> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<List<Faculty>> GetAllAsync(CancellationToken cancellationToken);
    void Add(Faculty entity);
    void Delete(Faculty entity);
}