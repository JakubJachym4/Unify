using MediatR;
using Unify.Application.Abstractions.Messaging;
using Unify.Domain.Abstractions;
using Unify.Domain.UniversityCore;
using Unify.Domain.UniversityCore.Abstractions;

namespace Unify.Application.Specializations;

public record ListSpecializationsQuery() : IQuery<List<SpecializationResponse>>;
public record GetSpecializationStudents(Guid SpecializationId) : IQuery<List<Guid>>;



internal sealed class ListSpecializationsQueryHandler : IQueryHandler<ListSpecializationsQuery, List<SpecializationResponse>>
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

internal sealed class GetSpecializationStudentsHandler : IQueryHandler<GetSpecializationStudents, List<Guid>>
{
    private readonly ISpecializationRepository _repository;

    public GetSpecializationStudentsHandler(ISpecializationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<Guid>>> Handle(GetSpecializationStudents request, CancellationToken cancellationToken)
    {
        var specialization = await _repository.GetByIdAsync(request.SpecializationId, cancellationToken);

        if (specialization == null)
        {
            return Result.Failure<List<Guid>>("SpecializationId.NotFound", "SpecializationId not found.");
        }

        return await _repository.GetStudentsGuidsAsync(specialization, cancellationToken);
    }
}

public record SpecializationResponse(Guid Id, string Name, string Description, Guid FieldOfStudyId);
