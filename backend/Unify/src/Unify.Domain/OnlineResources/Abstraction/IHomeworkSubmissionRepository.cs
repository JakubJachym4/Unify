namespace Unify.Domain.OnlineResources.Abstraction;

public interface IHomeworkSubmissionRepository
{
    Task<HomeworkSubmission?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<HomeworkSubmission>> GetAllAsync(CancellationToken cancellationToken);
    void Add(HomeworkSubmission entity);
    void Delete(HomeworkSubmission entity);
    Task<List<HomeworkSubmission>> GetByCourseAsync(HomeworkSubmission course, CancellationToken cancellationToken);
}