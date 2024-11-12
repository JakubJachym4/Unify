namespace Unify.Domain.Messages;

public interface IMessageRepository
{
    Task<Message?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ICollection<Message>> GetMultipleBySenderIdAsync(Guid senderId, CancellationToken cancellationToken = default);
    Task<ICollection<Message>> GetLastMultipleByDateAsync(Guid senderId, DateOnly date, CancellationToken cancellationToken = default);
    Task<ICollection<Message>> GetLastMultipleByNumberAsync(Guid senderId, int number, CancellationToken cancellationToken = default);
    void Add(Message user);
}