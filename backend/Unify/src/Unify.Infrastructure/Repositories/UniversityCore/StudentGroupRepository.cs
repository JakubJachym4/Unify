using Microsoft.EntityFrameworkCore;
using Unify.Domain.UniversityCore;
using Unify.Domain.UniversityCore.Abstractions;
using Unify.Domain.Users;

namespace Unify.Infrastructure.Repositories.UniversityCore;

public sealed class StudentGroupRepository : Repository<StudentGroup>, IStudentGroupRepository

{
    public StudentGroupRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public override Task<StudentGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DbContext.Set<StudentGroup>().Include(sg => sg.Members).FirstOrDefaultAsync(group => group.Id == id, cancellationToken);
    }

    public Task<StudentGroup?> GetByUserAsync(User user, CancellationToken cancellationToken = default)
    {
        return DbContext.Set<StudentGroup>().FirstOrDefaultAsync(group => group.Members.Any(student => student.Id == user.Id), cancellationToken);
    }

    public Task<List<StudentGroup>> GetGroupsBySpecializationAsync(Specialization specialization, CancellationToken cancellationToken = default)
    {
        return DbContext.Set<StudentGroup>().Include(sg => sg.Members).
            Where(group => group.SpecializationId == specialization.Id).ToListAsync(cancellationToken);
    }

    public Task AddManyAsync(IEnumerable<StudentGroup> entities, CancellationToken cancellationToken = default)
    {
        return DbContext.Set<StudentGroup>().AddRangeAsync(entities, cancellationToken);
    }
}