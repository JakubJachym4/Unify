using Unify.Domain.UniversityCore;
using Unify.Domain.UniversityCore.Abstractions;

namespace Unify.Infrastructure.Repositories.UniversityCore;

internal class MarkRepository : Repository<Mark>, IMarkRepository
{
    public MarkRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}