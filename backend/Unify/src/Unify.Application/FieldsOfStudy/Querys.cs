using MediatR;
using Unify.Application.Abstractions.Messaging;
using Unify.Domain.Abstractions;
using Unify.Domain.UniversityCore;
using Unify.Domain.UniversityCore.Abstractions;

namespace Unify.Application.FieldsOfStudy;

public record ListFieldOfStudiesQuery() : IQuery<List<FieldOfStudyResult>>;
public record FieldOfStudyResult(Guid Id, string Name, string Description, Guid FacultyId);


internal sealed class ListFieldOfStudiesQueryHandler : IQueryHandler<ListFieldOfStudiesQuery, List<FieldOfStudyResult>>
{
    private readonly IFieldOfStudyRepository _repository;

    public ListFieldOfStudiesQueryHandler(IFieldOfStudyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<FieldOfStudyResult>>> Handle(ListFieldOfStudiesQuery request, CancellationToken cancellationToken)
    {
        var fieldOfStudies = await _repository.GetAllAsync(cancellationToken);
        return Result.Success(fieldOfStudies.Select(f=> new FieldOfStudyResult(f.Id, f.Name.Value, f.Description.Value, f.FacultyId)).ToList());
    }
}