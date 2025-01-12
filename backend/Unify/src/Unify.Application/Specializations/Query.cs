using MediatR;
using Unify.Domain.Abstractions;
using Unify.Domain.UniversityCore;
using Unify.Domain.UniversityCore.Abstractions;

namespace Unify.Application.Specializations;

public record ListSpecializationsQuery() : IRequest<Result<List<SpecializationResponse>>>;

public record SpecializationResponse(Guid Id, string Name, string Description, Guid FieldOfStudyId);

internal sealed class ListSpecializationsQueryHandler : IRequestHandler<ListSpecializationsQuery, Result<List<SpecializationResponse>>>
{
    private readonly ISpecializationRepository _repository;

    public ListSpecializationsQueryHandler(ISpecializationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<SpecializationResponse>>> Handle(ListSpecializationsQuery request, CancellationToken cancellationToken)
    {
        var specializations = await _repository.GetAllAsync(cancellationToken);
        return Result.Success(
            specializations.Select(s => new SpecializationResponse(s.Id, s.Name.Value, s.Description.Value, s.FieldOfStudyId)).ToList()
            );
    }
}