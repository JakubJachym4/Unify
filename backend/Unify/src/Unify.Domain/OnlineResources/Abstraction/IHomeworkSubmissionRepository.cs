using Unify.Domain.Users;

namespace Unify.Domain.OnlineResources.Abstraction;

public interface IHomeworkSubmissionRepository
{
    Task<HomeworkSubmission?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<HomeworkSubmission>> GetAllAsync(CancellationToken cancellationToken);
    void Add(HomeworkSubmission entity);
    void Delete(HomeworkSubmission entity);
    Task<List<HomeworkSubmission>> GetByAssignmentAsync(HomeworkAssignment homeworkAssignment, CancellationToken cancellationToken);
    Task<List<HomeworkSubmission>> GetByStudentAsync(User student, CancellationToken cancellationToken);
}