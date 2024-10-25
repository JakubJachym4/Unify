using Microsoft.EntityFrameworkCore;
using Unify.Domain.Users;

namespace Unify.Infrastructure.Repositories;

internal sealed class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public User? GetByEmail(string email, CancellationToken cancellationToken = default)
    {
        return DbContext.Set<User>().AsEnumerable()
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