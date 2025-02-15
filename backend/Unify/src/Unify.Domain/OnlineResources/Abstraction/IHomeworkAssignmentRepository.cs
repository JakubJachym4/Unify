using Unify.Domain.UniversityClasses;
using Unify.Domain.Users;

namespace Unify.Domain.OnlineResources.Abstraction;

public interface IHomeworkAssignmentRepository
{
    Task<HomeworkAssignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<HomeworkAssignment>> GetAllAsync(CancellationToken cancellationToken);
    Task<List<HomeworkAssignment>> GetByClassOffering(ClassOffering classOffering, CancellationToken cancellationToken);
    void Add(HomeworkAssignment entity);
    void Delete(HomeworkAssignment entity);
}