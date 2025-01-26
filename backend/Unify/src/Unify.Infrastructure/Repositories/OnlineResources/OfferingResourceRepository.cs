using Microsoft.EntityFrameworkCore;
using Unify.Domain.OnlineResources;
using Unify.Domain.OnlineResources.Abstraction;
using Unify.Domain.UniversityClasses;

namespace Unify.Infrastructure.Repositories.OnlineResources;

internal class OfferingResourceRepository : Repository<OfferingResource>, IOfferingResourceRepository
{
    public OfferingResourceRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<OfferingResource?> GetByIdAsyncIncludeAttachments(Guid id, CancellationToken cancellationToken)
    {
        return DbContext.Set<OfferingResource>().Include(or => or.Files)
            .FirstOrDefaultAsync(or => or.Id == id, cancellationToken);
    }

    public Task<List<OfferingResource>> GetByClassOfferingAsync(ClassOffering offering, CancellationToken cancellationToken)
    {
        return DbContext.Set<OfferingResource>().Where(o => o.ClassOfferingId == offering.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<List<OfferingResource>> GetByClassOfferingAsyncIncludeAttachments(ClassOffering offering, CancellationToken cancellationToken)
    {
        return DbContext.Set<OfferingResource>().Include(or => or.Files)
            .Where(or => or.ClassOfferingId == offering.Id).ToListAsync(cancellationToken);
    }
}