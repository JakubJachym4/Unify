using Unify.Domain.UniversityClasses;

namespace Unify.Application.Lectures;

public record LectureResponse(Guid Id, Guid CourseId, string Title, DateTime ScheduledDate, TimeSpan Duration, Guid LecturerId, Guid LocationId)
{
    public static LectureResponse CreateFrom(Lecture lecture)
    {
        return new LectureResponse(lecture.Id, lecture.CourseId, lecture.Title.Value, lecture.ScheduledDate, lecture.Duration, lecture.LecturerId, lecture.LocationId);
    }
}