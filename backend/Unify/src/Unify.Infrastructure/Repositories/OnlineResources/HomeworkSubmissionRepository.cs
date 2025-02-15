using Microsoft.EntityFrameworkCore;
using Unify.Domain.OnlineResources;
using Unify.Domain.OnlineResources.Abstraction;
using Unify.Domain.Users;

namespace Unify.Infrastructure.Repositories.OnlineResources;

internal class HomeworkSubmissionRepository : Repository<HomeworkSubmission>, IHomeworkSubmissionRepository
{
    public HomeworkSubmissionRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<List<HomeworkSubmission>> GetByAssignmentAsync(HomeworkAssignment homeworkAssignment, CancellationToken cancellationToken)
    {
        return DbContext.Set<HomeworkSubmission>().Where(submission => submission.HomeworkAssigmentId == homeworkAssignment.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<List<HomeworkSubmission>> GetByStudentAsync(User student, CancellationToken cancellationToken)
    {
        return DbContext.Set<HomeworkSubmission>().Where(submission => submission.StudentId == student.Id)
            .ToListAsync(cancellationToken);
    }
}