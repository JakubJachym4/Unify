using MediatR;
using Unify.Application.Abstractions.Messaging;
using Unify.Domain.Abstractions;
using Unify.Domain.UniversityCore;
using Unify.Domain.UniversityCore.Abstractions;

namespace Unify.Application.FieldsOfStudy;

public record ListFieldOfStudiesQuery() : IQuery<List<FieldOfStudy>>;

internal sealed class ListFieldOfStudiesQueryHandler : IQueryHandler<ListFieldOfStudiesQuery, List<FieldOfStudy>>
{
    private readonly IFieldOfStudyRepository _repository;

    public ListFieldOfStudiesQueryHandler(IFieldOfStudyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<FieldOfStudy>>> Handle(ListFieldOfStudiesQuery request, CancellationToken cancellationToken)
    {
        var fieldOfStudies = await _repository.GetAllAsync(cancellationToken);
        return Result.Success(fieldOfStudies.ToList());
    }
}