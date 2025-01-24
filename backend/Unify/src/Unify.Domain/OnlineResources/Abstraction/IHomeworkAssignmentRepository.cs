namespace Unify.Domain.OnlineResources.Abstraction;

public interface IHomeworkAssignmentRepository
{
    Task<HomeworkAssigment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<HomeworkAssigment>> GetAllAsync(CancellationToken cancellationToken);
    void Add(HomeworkAssigment entity);
    void Delete(HomeworkAssigment entity);
    Task<List<HomeworkAssigment>> GetByCourseAsync(HomeworkAssigment course, CancellationToken cancellationToken);
}