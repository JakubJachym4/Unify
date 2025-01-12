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

    public async Task<FieldOfStudy?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        //TODO: fix sql injection
        var sql = $"SELECT * FROM fields_of_study WHERE Name = '{name}'";

        return await DbContext.Set<FieldOfStudy>().FromSqlRaw(sql).FirstOrDefaultAsync(cancellationToken);

    }
}