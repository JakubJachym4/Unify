using Unify.Domain.Shared;
using Unify.Domain.Users;

namespace Unify.Domain.UniversityClasses.Abstractions;

public interface IClassOfferingRepository
{
    Task<ClassOffering?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ClassOffering?> GetByIdAsyncIncludeEnrollments(Guid id, CancellationToken cancellationToken);
    Task<List<ClassOffering>> GetAllAsync(CancellationToken cancellationToken);
    void Add(ClassOffering entity);
    void Delete(ClassOffering entity);
    Task<List<ClassOffering>> GetByLecturerAsync(User lecturer, CancellationToken cancellationToken);
}