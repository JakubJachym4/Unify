using Microsoft.EntityFrameworkCore;
using Unify.Domain.UniversityClasses;
using Unify.Domain.UniversityCore.Abstractions;

namespace Unify.Infrastructure.Repositories.UniversityClasses;

public sealed class ClassOfferingSessionRepository : Repository<ClassOfferingSession>, IClassOfferingSessionRepository
{
    public ClassOfferingSessionRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<List<ClassOfferingSession>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return DbContext.Set<ClassOfferingSession>().ToListAsync(cancellationToken);
    }
}