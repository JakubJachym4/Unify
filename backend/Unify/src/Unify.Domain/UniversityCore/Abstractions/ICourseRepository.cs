using Unify.Domain.Shared;
using Unify.Domain.Users;

namespace Unify.Domain.UniversityCore.Abstractions;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Course>> GetAllAsync(CancellationToken cancellationToken);
    Task<List<Course>> GetAllBySpecializationAsync(Specialization specialization, CancellationToken cancellationToken = default);
    void Add(Course entity);
    void Delete(Course entity);
    Task<List<Course>> GetByLecturerIdAsync(User lecturer, CancellationToken cancellationToken);
}