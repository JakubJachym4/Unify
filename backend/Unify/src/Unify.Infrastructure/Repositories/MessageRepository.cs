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
        return await DbContext.Set<Message>().Where(m => m.SenderId == senderId).ToListAsync(cancellationToken);
    }
}