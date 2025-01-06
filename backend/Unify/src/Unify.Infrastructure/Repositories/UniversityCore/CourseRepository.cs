using Microsoft.EntityFrameworkCore;
using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;
using Unify.Domain.UniversityCore.Abstractions;

namespace Unify.Infrastructure.Repositories.UniversityCore;

public class CourseRepository : Repository<Course>, ICourseRepository
{
    public CourseRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return DbContext.Set<Course>().FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public Task<List<Course>> GetAllAsync(CancellationToken cancellationToken)
    {
        return DbContext.Set<Course>().ToListAsync(cancellationToken);
    }
}