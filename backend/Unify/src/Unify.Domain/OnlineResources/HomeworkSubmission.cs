using Unify.Domain.Abstractions;
using Unify.Domain.Users;

namespace Unify.Domain.OnlineResources;

public sealed class HomeworkSubmission : Entity
{
    public HomeworkSubmission(HomeworkAssigment homeworkAssigment, User student, DateTime submittedOn) : base(Guid.NewGuid())
    {
        HomeworkAssigment = homeworkAssigment;
        Student = student;
        SubmittedOn = submittedOn;
    }

    public HomeworkAssigment HomeworkAssigment { get; private set; }
    public User Student { get; private set; }
    public DateTime SubmittedOn { get; private set; }
    private readonly List<Attachment> _files = new();
    public IReadOnlyCollection<Attachment> Files => _files;
    
    public void AddFile(Attachment file) => _files.Add(file);
    public void AddFiles(IEnumerable<Attachment> files) => _files.AddRange(files);
    public void RemoveFile(Attachment file) => _files.Remove(file);
}