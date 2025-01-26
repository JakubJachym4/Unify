using Unify.Domain.OnlineResources;
using Unify.Domain.OnlineResources.Abstraction;

namespace Unify.Infrastructure.Repositories.OnlineResources;

internal class HomeworkAssignmentRepository : Repository<HomeworkAssigment>, IHomeworkAssignmentRepository
{
    public HomeworkAssignmentRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}