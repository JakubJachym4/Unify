namespace Unify.Domain.UniversityCore.Abstractions;

public interface IGradeRepository
{
    Task<Grade?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Grade>> GetAllAsync(CancellationToken cancellationToken);
    void Add(Grade entity);
    void Delete(Grade entity);
}