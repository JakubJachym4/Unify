using Unify.Domain.Abstractions;
using Unify.Domain.Messages;
using Unify.Domain.Shared;
using Unify.Domain.UniversityClasses;
using Unify.Domain.UniversityCore;
using Guid = System.Guid;

namespace Unify.Domain.OnlineResources;

public sealed class HomeworkAssigment : HomeworkBaseEntity
{
    private HomeworkAssigment() { }
    public HomeworkAssigment(ClassOffering classOffering, Title title, Description description, DateTime dueDate) : base(Guid.NewGuid())
    {
        ClassOfferingId = classOffering.Id;
        Title = title;
        Description = description;
        DueDate = dueDate;
    }

    public Guid ClassOfferingId { get; private set; }
    public Title Title { get; private set; }
    public Description Description { get; private set; }
    public DateTime DueDate { get; private set; }
    public bool Locked { get; private set; } = false;


    private readonly List<HomeworkSubmission> _submissions = new();
    private readonly List<Attachment> _attachments = new();
    public IReadOnlyCollection<HomeworkSubmission> Submissions => _submissions;
    public IReadOnlyCollection<Attachment> Attachments => _attachments;

    public void AddSubmission(HomeworkSubmission homeworkSubmission) => _submissions.Add(homeworkSubmission);
    public void AttachFile(Attachment attachment) => _attachments.Add(attachment);
    public void RemoveFile(Attachment attachment) => _attachments.Remove(attachment);
    public void LockSubmission() => Locked = true;
    public void UnlockSubmission() => Locked = false;
    public void ClearFiles() => _attachments.Clear();

    public void Update(Title title, Description description, DateTime requestDueDate)
    {
        Title = title;
        Description = description;
        DueDate = requestDueDate;
    }
}