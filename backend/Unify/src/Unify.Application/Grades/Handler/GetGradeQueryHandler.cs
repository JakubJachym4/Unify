using Unify.Application.Abstractions.Messaging;
using Unify.Application.ClassEnrollment;
using Unify.Application.Homework.HomeworkAssignments.Handlers;
using Unify.Domain.Abstractions;
using Unify.Domain.UniversityCore.Abstractions;

namespace Unify.Application.Grades.Handler;

public record GetGradeQuery(Guid Id) : IQuery<GradeResponse>;

public class GetGradeQueryHandler : IQueryHandler<GetGradeQuery, GradeResponse>
{
    private readonly IGradeRepository _gradeRepository;

    public GetGradeQueryHandler(IGradeRepository gradeRepository)
    {
        _gradeRepository = gradeRepository;
    }

    public async Task<Result<GradeResponse>> Handle(GetGradeQuery request, CancellationToken cancellationToken)
    {
        var grade = await _gradeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (grade == null)
        {
            return GradeErrors.NotFound;
        }

        return GradeResponse.Create(grade);
    }
}