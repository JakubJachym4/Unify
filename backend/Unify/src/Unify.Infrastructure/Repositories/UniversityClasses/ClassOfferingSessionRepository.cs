using Microsoft.EntityFrameworkCore;
using Unify.Domain.UniversityClasses;
using Unify.Domain.UniversityCore.Abstractions;

namespace Unify.Infrastructure.Repositories.UniversityClasses;

public sealed class ClassOfferingSessionRepository : Repository<ClassOfferingSession>, IClassOfferingSessionRepository
{
    public ClassOfferingSessionRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public override Task<List<ClassOfferingSession>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return DbContext.Set<ClassOfferingSession>().ToListAsync(cancellationToken);
    }

    public Task<List<ClassOfferingSession>> GetByClassOfferingIdAsync(Guid classOfferingId, CancellationToken cancellationToken)
    {
        return DbContext.Set<ClassOfferingSession>().Where(os => os.Id == classOfferingId)
            .ToListAsync(cancellationToken);
    }

    public Task<List<ClassOfferingSession>> GetByLecturerIdAsync(Guid lecturerId, CancellationToken cancellationToken)
    {
        return DbContext.Set<ClassOfferingSession>().Where(os => os.LecturerId == lecturerId)
            .ToListAsync(cancellationToken);
    }
}