using Microsoft.EntityFrameworkCore;
using Unify.Domain.UniversityCore;
using Unify.Domain.UniversityCore.Abstractions;

namespace Unify.Infrastructure.Repositories.UniversityCore;

public class LocationRepository : Repository<Location>, ILocationRepository
{
    public LocationRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return DbContext.Set<Location>().FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public Task<List<Location>> GetAllAsync(CancellationToken cancellationToken)
    {
        return DbContext.Set<Location>().ToListAsync(cancellationToken);
    }
}