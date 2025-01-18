using Unify.Domain.Shared;
using Unify.Domain.Users;

namespace Unify.Domain.UniversityCore.Abstractions;

public interface ISpecializationRepository
{
    Task<Specialization?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Specialization?> GetByNameAsync(Name name, CancellationToken cancellationToken);
    Task<List<Specialization>> GetAllAsync(CancellationToken cancellationToken);
    void Add(Specialization entity);
    void Delete(Specialization entity);

    Task<List<Guid>> GetStudentsGuidsAsync(Specialization specialization, CancellationToken cancellationToken);
    Task<List<User>> GetStudentsAsync(Specialization specialization, CancellationToken cancellationToken);

}