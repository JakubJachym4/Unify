namespace Unify.Domain.Messages;

public interface IMessageRepository
{
    Task<Message?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ICollection<Message>> GetMultipleBySenderIdAsync(Guid senderId, CancellationToken cancellationToken = default);
    Task<ICollection<Message>> GetLastMultipleBySenderAndDateAsync(Guid senderId, DateOnly date, CancellationToken cancellationToken = default);
    Task<ICollection<Message>> GetLastMultipleBySenderAndNumberAsync(Guid senderId, int number, CancellationToken cancellationToken = default);
    void Add(Message user);
}