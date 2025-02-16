using Microsoft.EntityFrameworkCore;
using Unify.Domain.UniversityCore;
using Unify.Domain.UniversityCore.Abstractions;

namespace Unify.Infrastructure.Repositories.UniversityCore;

internal class GradeRepository : Repository<Grade>, IGradeRepository
{
    public GradeRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public override Task<Grade?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DbContext.Set<Grade>().Include(g => g.Marks).FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }
}