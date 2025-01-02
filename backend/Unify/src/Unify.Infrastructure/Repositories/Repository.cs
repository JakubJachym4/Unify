using Microsoft.EntityFrameworkCore;
using Unify.Domain.Abstractions;

namespace Unify.Infrastructure.Repositories;

public abstract class Repository<T>
where T : Entity
{
    protected readonly ApplicationDbContext DbContext;

    protected Repository(ApplicationDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public virtual async Task<T?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<T>()
            .FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public virtual async Task<T?> GetByIdAsyncNoTracking(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<T>()
            .AsNoTracking()
            .FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public virtual async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<T>().ToListAsync(cancellationToken);
    }

    public virtual void Add(T entity)
    {
        DbContext.Add(entity);
    }

    public virtual void Delete(T entity)
    {
        DbContext.Set<T>().Remove(entity);
    }
}