namespace Unify.Domain.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ICollection<User>> GetManyByIdAsync(ICollection<Guid> ids, CancellationToken cancellationToken = default);
    User? GetByEmail(string email, CancellationToken cancellationToken = default);

    void Add(User user);

}