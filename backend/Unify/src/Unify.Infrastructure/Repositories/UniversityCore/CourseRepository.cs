using Microsoft.EntityFrameworkCore;
using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;
using Unify.Domain.UniversityCore.Abstractions;
using Unify.Domain.Users;

namespace Unify.Infrastructure.Repositories.UniversityCore;

public class CourseRepository : Repository<Course>, ICourseRepository
{
    public CourseRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public override Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DbContext.Set<Course>().FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public override Task<List<Course>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return DbContext.Set<Course>().ToListAsync(cancellationToken);
    }

    public Task<List<Course>> GetAllBySpecializationAsync(Specialization specialization,
        CancellationToken cancellationToken = default)
    {
        return DbContext.Set<Course>().Where(entity => entity.SpecializationId == specialization.Id).ToListAsync(cancellationToken);
    }

    public Task<List<Course>> GetByLecturerIdAsync(User lecturer, CancellationToken cancellationToken)
    {
        return DbContext.Set<Course>().Include(c => c.Classes)
            .Where(course => course.LecturerId == lecturer.Id).ToListAsync(cancellationToken);
    }
}