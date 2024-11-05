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


    public async Task<ICollection<Message>> GetLastMultipleBySenderAndDateAsync(Guid senderId, DateOnly date, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<Message>()
            .Include(message => message.Recipients)
            .Include(message => message.Attachments)
            .Where(m => m.SenderId == senderId && DateOnly.FromDateTime(m.CreatedOn) >= date)
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<Message>> GetLastMultipleBySenderAndNumberAsync(Guid senderId, int number, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<Message>()
            .Include(message => message.Recipients)
            .Include(message => message.Attachments)
            .Where(m => m.SenderId == senderId)
            .OrderByDescending(m => m.CreatedOn)
            .Take(number)
            .ToListAsync(cancellationToken);
    }
}