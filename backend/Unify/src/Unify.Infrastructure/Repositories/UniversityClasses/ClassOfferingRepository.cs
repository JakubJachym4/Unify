using Microsoft.EntityFrameworkCore;
using Unify.Domain.Shared;
using Unify.Domain.UniversityClasses;
using Unify.Domain.UniversityClasses.Abstractions;
using Unify.Domain.Users;

namespace Unify.Infrastructure.Repositories.UniversityClasses;

public class ClassOfferingRepository : Repository<ClassOffering>, IClassOfferingRepository
{
    public ClassOfferingRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public override Task<ClassOffering?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return DbContext
            .Set<ClassOffering>()
            .FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public Task<ClassOffering?> GetByIdAsyncIncludeEnrollments(Guid id, CancellationToken cancellationToken)
    {
        return DbContext.Set<ClassOffering>().Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public Task<List<ClassOffering>> GetByLecturerAsync(User lecturer, CancellationToken cancellationToken)
    {
        return DbContext.Set<ClassOffering>().Include(c => c.Enrollments)
            .Where(course => course.LecturerId == lecturer.Id).ToListAsync(cancellationToken);
    }
}