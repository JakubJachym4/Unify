using Unify.Domain.UniversityClasses;

namespace Unify.Domain.OnlineResources.Abstraction;

public interface IOfferingResourceRepository
{
    Task<OfferingResource?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<OfferingResource>> GetAllAsync(CancellationToken cancellationToken);
    void Add(OfferingResource entity);
    void Delete(OfferingResource entity);
    Task<List<OfferingResource>> GetByClassOfferingAsync(ClassOffering course, CancellationToken cancellationToken);
}