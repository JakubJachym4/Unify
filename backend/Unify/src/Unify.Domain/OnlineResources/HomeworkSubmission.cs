using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;
using Unify.Domain.Users;
using Guid = System.Guid;

namespace Unify.Domain.OnlineResources;

public sealed class HomeworkSubmission : HomeworkBaseEntity
{
    private HomeworkSubmission() { }
    private HomeworkSubmission(Guid id, Guid homeworkAssigmentId, Guid studentId, DateTime submittedOn) : base(id)
    {
        HomeworkAssigmentId = homeworkAssigmentId;
        StudentId = studentId;
        SubmittedOn = submittedOn;
    }

    public Guid HomeworkAssigmentId { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid? MarkId { get; private set; }
    public TextContent? Feedback { get; private set; }
    public DateTime SubmittedOn { get; private set; }

    private readonly List<Attachment> _attachments = new List<Attachment>();
    public IReadOnlyCollection<Attachment> Attachments => _attachments;

    public void AddAttachments(IEnumerable<Attachment> attachments) => _attachments.AddRange(attachments);

    public static HomeworkSubmission Submit(HomeworkAssigment assigment, User student, DateTime submittedOn,
        ICollection<Attachment>? attachments = null)
    {
        var submission = new HomeworkSubmission(Guid.NewGuid(), assigment.Id, student.Id, submittedOn);
        if (attachments != null && attachments.Any())
        {
            submission.AddAttachments(attachments);
        }
        return submission;
    }

    public Grade AddMark(Grade grade, Mark mark, TextContent? feedback = null)
    {
        MarkId = mark.Id;
        grade.AddMark(mark);
        Feedback = feedback;
        return grade;
    }

    public void ClearFiles() => _attachments.Clear();

    public void Update(DateTime requestSubmittedOn) => SubmittedOn = requestSubmittedOn;

    public void ProvideFeedback(TextContent textContent)
    {
        Feedback = textContent;
    }
}