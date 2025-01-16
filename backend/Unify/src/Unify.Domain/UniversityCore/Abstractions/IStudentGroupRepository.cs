using Unify.Domain.Shared;
using Unify.Domain.Users;

namespace Unify.Domain.UniversityCore.Abstractions;

public interface IStudentGroupRepository
{
    Task<StudentGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<StudentGroup>> GetGroupsBySpecializationAsync(Specialization specialization, CancellationToken cancellationToken = default);
    Task<StudentGroup?> GetByUserAsync(User user, CancellationToken cancellationToken = default);
    Task<List<StudentGroup>> GetAllAsync(CancellationToken cancellationToken = default);
    void Add(StudentGroup entity);
    Task AddManyAsync(IEnumerable<StudentGroup> entities, CancellationToken cancellationToken = default);
    void Delete(StudentGroup entity);
}