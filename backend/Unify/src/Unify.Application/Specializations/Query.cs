using MediatR;
using Unify.Domain.Abstractions;
using Unify.Domain.UniversityCore;
using Unify.Domain.UniversityCore.Abstractions;

namespace Unify.Application.Specializations;

public record ListSpecializationsQuery() : IRequest<Result<List<Specialization>>>;

internal sealed class ListSpecializationsQueryHandler : IRequestHandler<ListSpecializationsQuery, Result<List<Specialization>>>
{
    private readonly ISpecializationRepository _repository;

    public ListSpecializationsQueryHandler(ISpecializationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<Specialization>>> Handle(ListSpecializationsQuery request, CancellationToken cancellationToken)
    {
        var specializations = await _repository.GetAllAsync(cancellationToken);
        return Result.Success(specializations.ToList());
    }
}