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
        return DbContext.Set<HomeworkSubmission>().Include(s => s.Attachments).Where(submission => submission.HomeworkAssigmentId == homeworkAssignment.Id)
            .ToListAsync(cancellationToken);
    }

    public override Task<HomeworkSubmission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DbContext.Set<HomeworkSubmission>().Include(s => s.Attachments).FirstOrDefaultAsync(submission => submission.Id == id, cancellationToken);
    }

    public Task<List<HomeworkSubmission>> GetByStudentAsync(User student, CancellationToken cancellationToken)
    {
        return DbContext.Set<HomeworkSubmission>().Include(s => s.Attachments).Where(submission => submission.StudentId == student.Id)
            .ToListAsync(cancellationToken);
    }
}