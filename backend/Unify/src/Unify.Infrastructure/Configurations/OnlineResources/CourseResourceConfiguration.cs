using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.OnlineResources;
using Unify.Domain.UniversityCore;
using Guid = System.Guid;

namespace Unify.Infrastructure.Configurations.OnlineResources;

internal sealed class CourseResourceConfiguration : OnlineResourceConfiguration<CourseResource>
{
    public override void Configure(EntityTypeBuilder<CourseResource> builder)
    {
        builder.ToTable("course_resources");

        builder.HasOne<Course>()
            .WithMany()
            .HasForeignKey(cr => cr.CourseId);

        base.Configure(builder);
    }
}