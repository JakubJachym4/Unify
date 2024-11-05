using Microsoft.EntityFrameworkCore;
using Unify.Domain.Users;

namespace Unify.Infrastructure.Repositories;

internal sealed class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<ICollection<User>> GetManyByIdAsync(ICollection<Guid> ids, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<User>().Where(u => ids.Contains(u.Id)).ToListAsync(cancellationToken);
    }

    public User? GetByEmailNoTracking(string email, CancellationToken cancellationToken = default)
    {
        return DbContext.Set<User>().AsNoTracking().AsEnumerable()
            .FirstOrDefault(user => user.Email.Value == email);
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