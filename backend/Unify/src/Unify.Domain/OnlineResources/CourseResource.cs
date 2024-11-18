using Unify.Domain.Messages;
using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;

namespace Unify.Domain.OnlineResources;

public sealed class CourseResource : OnlineResource
{
    private CourseResource() { }
    public CourseResource(Title title, Description description, Course course) : base(Guid.NewGuid(), title, description)
    {
        Course = course;
    }

    public Course Course { get; private set; }
}