using Microsoft.EntityFrameworkCore;
using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;
using Unify.Domain.UniversityCore.Abstractions;

namespace Unify.Infrastructure.Repositories.UniversityCore;

public class SpecializationRepository : Repository<Specialization>, ISpecializationRepository
{
    public SpecializationRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<Specialization?> GetByNameAsync(Name name, CancellationToken cancellationToken)
    {
        return DbContext.Set<Specialization>().FirstOrDefaultAsync(entity => entity.Name == name, cancellationToken);
    }

    public Task<List<Guid>> GetStudentsAsync(Guid specializationId, CancellationToken cancellationToken)
    {
        return DbContext.Set<Specialization>()
            .Include(s => s.Students)
            .FirstOrDefaultAsync(s => s.Id == specializationId, cancellationToken).ContinueWith(task =>
            {
                var specialization = task.Result;
                return specialization?.Students.Select(s => s.Id).ToList() ?? new List<Guid>();
            }, cancellationToken);
    }
}