using Unify.Domain.UniversityClasses;
using Unify.Domain.UniversityCore;
using Unify.Domain.Users;

namespace Unify.Domain.OnlineResources.Abstraction;

public interface ICourseResourceRepository
{
    Task<CourseResource?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<CourseResource>> GetAllAsync(CancellationToken cancellationToken);
    void Add(CourseResource entity);
    void Delete(CourseResource entity);
    Task<List<CourseResource>> GetByCourseAsync(Course course, CancellationToken cancellationToken);
}