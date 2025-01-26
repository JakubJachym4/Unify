using Microsoft.EntityFrameworkCore;
using Unify.Domain.UniversityClasses;
using Unify.Domain.UniversityClasses.Abstractions;

namespace Unify.Infrastructure.Repositories.UniversityClasses;

public sealed class ClassEnrollmentRepository : Repository<ClassEnrollment>, IClassEnrollmentRepository
{
    public ClassEnrollmentRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<List<ClassEnrollment>> GetByClassOfferingIdAsync(Guid classOfferingId, CancellationToken cancellationToken = default)
    {
        return DbContext.Set<ClassEnrollment>().Where(e => e.ClassOfferingId == classOfferingId).ToListAsync(cancellationToken);
    }

    public Task<List<ClassEnrollment>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return DbContext.Set<ClassEnrollment>().Where(e => e.StudentId == studentId).ToListAsync(cancellationToken);
    }

    public Task<ClassEnrollment?> GetByClassOfferingAndStudentAsync(Guid classOfferingId, Guid studentId, CancellationToken cancellationToken)
    {
        return DbContext.Set<ClassEnrollment>().FirstOrDefaultAsync(e => e.ClassOfferingId == classOfferingId && e.StudentId == studentId, cancellationToken);
    }
}