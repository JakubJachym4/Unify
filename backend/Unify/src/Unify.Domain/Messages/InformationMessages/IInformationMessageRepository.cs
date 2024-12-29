namespace Unify.Domain.Messages.InformationMessages;

public interface IInformationMessageRepository
{
    Task<ICollection<InformationMessage>> GetNonExpired(DateTime dateNow, CancellationToken cancellationToken = default);
    void Add(InformationMessage message);
}