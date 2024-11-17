using Unify.Domain.Abstractions;
using Unify.Domain.Messages;
using Unify.Domain.Users;

namespace Unify.Domain.FieldsOfStudy;

public sealed class FieldOfStudy : Entity
{
    public Name Name { get; private set; }
    public Description Description { get; private set; }
    public Faculty Faculty { get; private set; }

    private readonly List<Specialization> _specializations = new();
    public IReadOnlyCollection<Specialization> Specializations => _specializations;
}

public sealed class Specialization : Entity
{
    private readonly List<StudentGroup> _groups = new();

    public Specialization(Guid id, Name name, Description description, FieldOfStudy fieldOfStudy) : base(id)
    {
        Name = name;
        Description = description;
        FieldOfStudy = fieldOfStudy;
    }

    public Name Name { get; private set; }
    public Description Description { get; private set; }
    public FieldOfStudy FieldOfStudy { get; private set; }

    public IReadOnlyCollection<StudentGroup> Groups => _groups;

    public void AddGroup(StudentGroup group) => _groups.Add(group);
}

public sealed class Faculty : Entity
{
    public Name Name { get; private set; }
}

public sealed class StudentGroup : Entity
{
    private readonly List<User> _members = new();
    private readonly List<ClassEnrollment> _classEnrollments = new();

    public Name Name { get; private set; }
    public Specialization Specialization { get; private set; }
    public StudyYear StudyYear { get; private set; }
    public Semester Semester { get; private set; }
    public Term Term { get; private set; }


    public IReadOnlyCollection<User> Members => _members;
    public IReadOnlyCollection<ClassEnrollment> ClassEnrollments => _classEnrollments;

}

public sealed class ClassOffering : Entity
{
    private readonly List<ClassEnrollment> _enrollments = new();
    private readonly List<Message> _messages = new();
    public IReadOnlyCollection<ClassEnrollment> Enrollments => _enrollments;
    public IReadOnlyCollection<Message> Messages => _messages;

    public Course Course { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public User Instructor { get; private set; }

    public ClassOffering(Course course, DateOnly startDate, DateOnly endDate, User instructor) : base(Guid.NewGuid())
    {
        Course = course;
        StartDate = startDate;
        EndDate = endDate;
        Instructor = instructor;
    }

    public Result Enroll(User student, DateTime enrollmentDate)
    {
        if (_enrollments.Any(e => e.Student.Id == student.Id))
        {
            return Result.Failure(ClassOfferingErrors.AlreadyEnrolled(student.Id));
        }

        _enrollments.Add(ClassEnrollment.Enroll(this, student, enrollmentDate));

        return Result.Success();
    }
}

public sealed class Course : Entity
{
    public Course(Name name, Description description, Specialization specialization) : base(Guid.NewGuid())
    {
        Name = name;
        Description = description;
        Specialization = specialization;
    }

    public Name Name { get; private set; }
    public Description Description { get; private set; }
    public Specialization Specialization { get; private set; }

    private readonly List<ClassOffering> _classes = new();
    public IReadOnlyCollection<ClassOffering> Classes => _classes;

    public void AddClass(ClassOffering offering) => _classes.Add(offering);
}

public sealed class CourseResource : Entity
{
    public Course Course { get; private set; }
    public Attachment Attachment { get; private set; }
}

public sealed class ClassSession : Entity
{
    public ClassOffering ClassOffering { get; private set; }
    public ClassType ClassType { get; private set; }
    public DateTime ScheduledDate { get; private set; }
    public TimeSpan Duration { get; private set; }
    public User Instructor { get; private set; }
    public Location Location { get; private set; }
}

public enum ClassType
{
    Lecture = 0,
    Laboratory = 1,
}