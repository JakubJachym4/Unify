using Microsoft.EntityFrameworkCore;
using Unify.Domain.Messages;

namespace Unify.Infrastructure.Repositories;

internal sealed class MessageRepository : Repository<Message>, IMessageRepository
{
    public MessageRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<ICollection<Message>> GetMultipleBySenderIdAsync(Guid senderId, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<Message>()
            .Include(message => message.Recipients)
            .Include(message => message.Attachments)
            .Where(m => m.SenderId == senderId).ToListAsync(cancellationToken);
    }


    public async Task<ICollection<Message>> GetLastMultipleByDateAsync(Guid senderId, DateOnly date, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<Message>()
            .Include(message => message.Recipients)
            .Include(message => message.Attachments)
            .Where(m => (m.SenderId == senderId || m.Recipients.Any(recipient => recipient.Id == senderId)) && DateOnly.FromDateTime(m.CreatedOn) >= date)
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<Message>> GetLastMultipleByNumberAsync(Guid senderId, int number, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<Message>()
            .Include(message => message.Recipients)
            .Include(message => message.Attachments)
            .Where(m => m.SenderId == senderId || m.Recipients.Any(recipient => recipient.Id == senderId))
            .OrderByDescending(m => m.CreatedOn)
            .Take(number)
            .ToListAsync(cancellationToken);
    }
}