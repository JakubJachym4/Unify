using Unify.Domain.Shared;

namespace Unify.Domain.UniversityCore.Abstractions;

public interface ILocationRepository
{
    Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Location>> GetAllAsync(CancellationToken cancellationToken);
    void Add(Location entity);
    void Delete(Location entity);
}