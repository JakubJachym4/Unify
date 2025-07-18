﻿using Unify.Domain.Messages;
using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;
using Guid = System.Guid;

namespace Unify.Domain.OnlineResources;

public sealed class CourseResource : OnlineResource
{
    private CourseResource() { }
    public CourseResource(Title title, Description description, Course course) : base(Guid.NewGuid(), title, description)
    {
        CourseId = course.Id;
    }

    public static CourseResource Create(Title title, Description description, Course course)
    {
        return new CourseResource(title, description, course);
    }

    public Guid CourseId { get; private set; }
}