using Unify.Domain.Abstractions;
using Unify.Domain.Shared;
using Unify.Domain.UniversityClasses;

namespace Unify.Domain.OnlineResources;

public sealed class HomeworkAssigment : Entity
{
    public HomeworkAssigment(ClassOffering classOffering, Title ditle, Description description, DateTime dueDate) : base(Guid.NewGuid())
    {
        ClassOffering = classOffering;
        Ditle = ditle;
        Description = description;
        DueDate = dueDate;
    }

    public ClassOffering ClassOffering { get; private set; }
    public Title Ditle { get; private set; }
    public Description Description { get; private set; }
    public DateTime DueDate { get; private set; }
    public bool Locked { get; private set; } = false;


    private readonly List<HomeworkSubmission> _homeworkSubmissions = new();
    private readonly List<Attachment> _files = new();
    public IReadOnlyCollection<HomeworkSubmission> HomeworkSubmissions => _homeworkSubmissions;
    public IReadOnlyCollection<Attachment> Files => _files;

    public void AddSubmission(HomeworkSubmission homeworkSubmission) => _homeworkSubmissions.Add(homeworkSubmission);
    public void AttachFile(Attachment attachment) => _files.Add(attachment);
    public void RemoveFile(Attachment attachment) => _files.Remove(attachment);
    public void LockSubmission() => Locked = true;
    public void UnlockSubmission() => Locked = false;
}