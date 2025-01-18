using Microsoft.EntityFrameworkCore;
using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;
using Unify.Domain.UniversityCore.Abstractions;
using Unify.Domain.Users;

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

    public Task<List<Guid>> GetStudentsGuidsAsync(Specialization specialization, CancellationToken cancellationToken)
    {
        return DbContext.Set<Specialization>()
            .Include(s => s.Students)
            .FirstOrDefaultAsync(s => s.Id == specialization.Id, cancellationToken).ContinueWith(task =>
            {
                var specialization = task.Result;
                return specialization?.Students.Select(s => s.Id).ToList() ?? new List<Guid>();
            }, cancellationToken);
    }

    public Task<List<User>> GetStudentsAsync(Specialization specialization, CancellationToken cancellationToken)
    {
        return DbContext.Set<Specialization>()
            .Include(s => s.Students)
            .FirstOrDefaultAsync(s => s.Id == specialization.Id, cancellationToken)
            .ContinueWith(task =>
            {
                var specialization = task.Result;
                return specialization?.Students.ToList() ?? new List<User>();
            }, cancellationToken);
    }
}