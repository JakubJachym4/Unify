using Unify.Domain.UniversityCore;
using Unify.Domain.UniversityCore.Abstractions;

namespace Unify.Infrastructure.Repositories.UniversityCore;

internal class GradeRepository : Repository<Grade>, IGradeRepository
{
    public GradeRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

}