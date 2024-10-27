namespace Unify.Domain.Messages;

public interface IMessageRepository
{
    Task<Message?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ICollection<Message>> GetMultipleBySenderIdAsync(Guid senderId, CancellationToken cancellationToken = default);
    void Add(Message user);
}