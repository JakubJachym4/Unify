using Unify.Domain.UniversityClasses;

namespace Unify.Domain.UniversityCore.Abstractions;

public interface ILectureRepository
{
    Task<List<Lecture>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Lecture?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    void Add(Lecture entity);
    void Delete(Lecture entity);
    Task<List<Lecture>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken);
    Task<List<Lecture>> GetByLecturerIdAsync(Guid lecturerId, CancellationToken cancellationToken);
}