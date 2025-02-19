using Unify.Application.Abstractions.Clock;
using Unify.Application.Abstractions.Messaging;
using Unify.Application.ClassEnrollment;
using Unify.Application.Grades.CommandsAndQueries;
using Unify.Application.Homework.HomeworkAssignments.Handlers;
using Unify.Domain.Abstractions;
using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;
using Unify.Domain.UniversityCore.Abstractions;

namespace Unify.Application.Grades.Handler;

public class GradeHandlers : IQueryHandler<GetGradeQuery, GradeResponse>
{
    private readonly IGradeRepository _gradeRepository;

    public GradeHandlers(IGradeRepository gradeRepository)
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

public sealed class CreateMarkCommandHandler : ICommandHandler<CreateMarkCommand>
{
    private readonly IGradeRepository _gradeRepository;
    private readonly IMarkRepository _markRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateMarkCommandHandler(IGradeRepository gradeRepository, IMarkRepository markRepository, IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _gradeRepository = gradeRepository;
        _markRepository = markRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result> Handle(CreateMarkCommand request, CancellationToken cancellationToken)
    {
        var grade = await _gradeRepository.GetByIdAsync(request.GradeId, cancellationToken);
        if (grade == null)
        {
            return Result.Failure(GradeErrors.NotFound);
        }
        if(grade.DateAwarded != null)
        {
            return Result.Failure(GradeErrors.AlreadyAwarded);
        }

        var date = _dateTimeProvider.UtcNow;

        var mark = Mark.CreateForGrade(new Title(request.Title), grade, request.Score, request.MaxScore, date);

        grade.AddMark(mark);

        _markRepository.Add(mark);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public sealed class AwardGradeCommandHandler : ICommandHandler<AwardGradeCommand>
{
    private readonly IGradeRepository _gradeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AwardGradeCommandHandler(IGradeRepository gradeRepository, IUnitOfWork unitOfWork)
    {
        _gradeRepository = gradeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AwardGradeCommand request, CancellationToken cancellationToken)
    {
        var grade = await _gradeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (grade == null)
        {
            return Result.Failure(GradeErrors.NotFound);
        }

        if (request.Awarded)
        {
            grade.SetDateAwarded(DateTime.UtcNow);
        }
        else
        {
            grade.RevokeGradeAwarding();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}