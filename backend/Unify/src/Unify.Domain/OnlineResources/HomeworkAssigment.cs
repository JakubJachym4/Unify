using Unify.Domain.Abstractions;
using Unify.Domain.Messages;
using Unify.Domain.Shared;
using Unify.Domain.UniversityClasses;
using Unify.Domain.UniversityCore;

namespace Unify.Domain.OnlineResources;

public sealed class HomeworkAssigment : Entity
{
    public HomeworkAssigment(ClassOffering classOffering, Title title, Description description, DateTime dueDate) : base(Guid.NewGuid())
    {
        ClassOffering = classOffering;
        Title = title;
        Description = description;
        DueDate = dueDate;
    }

    public ClassOffering ClassOffering { get; private set; }
    public Title Title { get; private set; }
    public Description Description { get; private set; }
    public DateTime DueDate { get; private set; }
    public bool Locked { get; private set; } = false;
    public Mark? Mark { get; private set; } = null;
    public TextContent? Feedback { get; private set; } = null;


    private readonly List<HomeworkSubmission> _submissions = new();
    private readonly List<Attachment> _files = new();
    public IReadOnlyCollection<HomeworkSubmission> Submissions => _submissions;
    public IReadOnlyCollection<Attachment> Files => _files;

    public void AddSubmission(HomeworkSubmission homeworkSubmission) => _submissions.Add(homeworkSubmission);
    public void AttachFile(Attachment attachment) => _files.Add(attachment);
    public void RemoveFile(Attachment attachment) => _files.Remove(attachment);
    public void LockSubmission() => Locked = true;
    public void UnlockSubmission() => Locked = false;

    public void AddMark(Grade grade, Mark mark, TextContent? feedback = null)
    {
        Mark = mark;
        grade.AddMark(Mark);
        Feedback = feedback;
    }
}