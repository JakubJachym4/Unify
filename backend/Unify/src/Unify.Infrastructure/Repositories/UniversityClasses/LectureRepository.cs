using Microsoft.EntityFrameworkCore;
using Unify.Domain.UniversityClasses;
using Unify.Domain.UniversityCore.Abstractions;

namespace Unify.Infrastructure.Repositories.UniversityClasses;

public sealed class LectureRepository : Repository<Lecture>, ILectureRepository
{
    public LectureRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<List<Lecture>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return DbContext.Set<Lecture>().ToListAsync(cancellationToken);
    }

    public Task<List<Lecture>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken)
    {
        return DbContext.Set<Lecture>().Where(x => x.CourseId == courseId).ToListAsync(cancellationToken);
    }

    public Task<List<Lecture>> GetByLecturerIdAsync(Guid lecturerId, CancellationToken cancellationToken)
    {
        return DbContext.Set<Lecture>().Where(l => l.LecturerId == lecturerId).ToListAsync(cancellationToken);
    }
}