using Unify.Domain.Shared;

namespace Unify.Domain.UniversityCore.Abstractions;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Course>> GetAllAsync(CancellationToken cancellationToken);
    void Add(Course entity);
    void Delete(Course entity);
}