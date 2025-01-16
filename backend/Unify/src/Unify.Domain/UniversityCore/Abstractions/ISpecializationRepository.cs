using Unify.Domain.Shared;

namespace Unify.Domain.UniversityCore.Abstractions;

public interface ISpecializationRepository
{
    Task<Specialization?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Specialization?> GetByNameAsync(Name name, CancellationToken cancellationToken);
    Task<List<Specialization>> GetAllAsync(CancellationToken cancellationToken);
    void Add(Specialization entity);
    void Delete(Specialization entity);

    Task<List<Guid>> GetStudentsAsync(Guid specializationId, CancellationToken cancellationToken);

}