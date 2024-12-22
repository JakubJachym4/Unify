namespace Unify.Domain.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ICollection<User>> GetManyByIdAsync(ICollection<Guid> ids, CancellationToken cancellationToken = default);
    Task<ICollection<User>> GetAllAsync(CancellationToken cancellationToken = default);
    User? GetByEmailNoTracking(string email, CancellationToken cancellationToken = default);

    void Update(User user);

    void Add(User user);

}