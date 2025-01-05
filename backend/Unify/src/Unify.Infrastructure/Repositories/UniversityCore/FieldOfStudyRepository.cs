using Microsoft.EntityFrameworkCore;
using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;
using Unify.Domain.UniversityCore.Abstractions;

namespace Unify.Infrastructure.Repositories.UniversityCore;

public class FieldOfStudyRepository : Repository<FieldOfStudy>, IFieldOfStudyRepository
{
    public FieldOfStudyRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<FieldOfStudy?> GetByNameAsync(Name name, CancellationToken cancellationToken)
    {
        return DbContext.Set<FieldOfStudy>().FirstOrDefaultAsync(entity => entity.Name == name, cancellationToken);
    }
}