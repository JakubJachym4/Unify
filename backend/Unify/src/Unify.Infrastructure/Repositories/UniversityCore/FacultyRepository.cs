using Microsoft.EntityFrameworkCore;
using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;
using Unify.Domain.UniversityCore.Abstractions;

namespace Unify.Infrastructure.Repositories.UniversityCore;

public class FacultyRepository : Repository<Faculty>, IFacultyRepository
{
    public FacultyRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Faculty? GetByName(string name, CancellationToken cancellationToken)
    {
        return DbContext.Set<Faculty>()
            .AsEnumerable()
            .FirstOrDefault(entity => entity.Name.Value == name);
    }
}