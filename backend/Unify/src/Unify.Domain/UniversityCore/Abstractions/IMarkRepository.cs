namespace Unify.Domain.UniversityCore.Abstractions;

public interface IMarkRepository
{
    Task<Mark?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Mark>> GetAllAsync(CancellationToken cancellationToken);
    void Add(Mark entity);
    void Delete(Mark entity);
}