using Microsoft.EntityFrameworkCore;
using Unify.Domain.Messages.InformationMessages;

namespace Unify.Infrastructure.Repositories;

internal class InformationMessageRepository : Repository<InformationMessage>, IInformationMessageRepository
{
    public InformationMessageRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<ICollection<InformationMessage>> GetNonExpired(DateTime dateNow, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<InformationMessage>()
            .Include(message => message.Recipients)
            .Include(message => message.Attachments)
            .Where(message => message.ExpirationDate > dateNow)
            .ToListAsync(cancellationToken);
    }
}