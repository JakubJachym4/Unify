using Microsoft.EntityFrameworkCore;
using Unify.Domain.Users;

namespace Unify.Infrastructure.Repositories;

internal sealed class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public override async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<User>()
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<ICollection<User>> GetManyByIdAsync(ICollection<Guid> ids, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<User>().Where(u => ids.Contains(u.Id)).ToListAsync(cancellationToken);
    }

    public async Task<ICollection<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<User>().Include(user => user.Roles).ToListAsync(cancellationToken);
    }

    public User? GetByEmailNoTracking(string email, CancellationToken cancellationToken = default)
    {
        return DbContext.Set<User>().AsNoTracking().AsEnumerable()
            .FirstOrDefault(user => user.Email.Value == email);
    }

    public async Task<bool> AddRole(User user, Role role, CancellationToken cancellationToken = default)
    {
        var dbRole = await DbContext.Set<Role>().FirstOrDefaultAsync(r => r.Id == role.Id, cancellationToken: cancellationToken);
        if (dbRole == null)
        {
            return false;
        }
        user.AddRole(dbRole);
        return true;
    }

    public async void Update(User user)
    {
        DbContext.Set<User>().Update(user);
    }

    public override void Add(User user)
    {

        foreach (var role in user.Roles)
        {
            DbContext.Attach(role);
        }

        DbContext.Add(user);
    }
}