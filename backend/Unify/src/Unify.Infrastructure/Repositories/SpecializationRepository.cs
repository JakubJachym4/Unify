using Microsoft.EntityFrameworkCore;
using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;
using Unify.Domain.UniversityCore.Abstractions;

namespace Unify.Infrastructure.Repositories;

public class SpecializationRepository : Repository<Specialization>, ISpecializationRepository
{
    public SpecializationRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<Specialization?> GetByNameAsync(Name name, CancellationToken cancellationToken)
    {
        return DbContext.Set<Specialization>().FirstOrDefaultAsync(entity => entity.Name == name, cancellationToken);
    }
}