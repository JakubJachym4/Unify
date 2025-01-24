using Microsoft.EntityFrameworkCore;
using Unify.Domain.OnlineResources;
using Unify.Domain.OnlineResources.Abstraction;
using Unify.Domain.UniversityCore;
using Unify.Domain.Users;

namespace Unify.Infrastructure.Repositories.OnlineResources;

public class CourseResourceRepository : Repository<CourseResource>, ICourseResourceRepository
{
    public CourseResourceRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<List<CourseResource>> GetByCourseAsync(Course course, CancellationToken cancellationToken)
    {
        return DbContext.Set<CourseResource>().Where(cr => cr.CourseId == course.Id).ToListAsync(cancellationToken);
    }
}