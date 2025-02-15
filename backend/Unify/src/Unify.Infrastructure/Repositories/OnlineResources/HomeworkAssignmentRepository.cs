using Microsoft.EntityFrameworkCore;
using Unify.Domain.OnlineResources;
using Unify.Domain.OnlineResources.Abstraction;
using Unify.Domain.UniversityClasses;
using Unify.Domain.Users;

namespace Unify.Infrastructure.Repositories.OnlineResources;

internal class HomeworkAssignmentRepository : Repository<HomeworkAssignment>, IHomeworkAssignmentRepository
{
    public HomeworkAssignmentRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public override Task<HomeworkAssignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DbContext.Set<HomeworkAssignment>().Include(assignment => assignment.Attachments).FirstOrDefaultAsync(assignment => assignment.Id == id);

    }

    public Task<List<HomeworkAssignment>> GetByClassOffering(ClassOffering classOffering, CancellationToken cancellationToken)
    {
        return DbContext.Set<HomeworkAssignment>().Include(assignment => assignment.Attachments).Where(assignment => assignment.ClassOfferingId == classOffering.Id)
            .ToListAsync(cancellationToken);
    }
}