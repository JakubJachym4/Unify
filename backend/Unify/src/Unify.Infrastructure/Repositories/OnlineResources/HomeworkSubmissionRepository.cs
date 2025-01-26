using Unify.Domain.OnlineResources;
using Unify.Domain.OnlineResources.Abstraction;

namespace Unify.Infrastructure.Repositories.OnlineResources;

internal class HomeworkSubmissionRepository : Repository<HomeworkSubmission>, IHomeworkSubmissionRepository
{
    public HomeworkSubmissionRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}