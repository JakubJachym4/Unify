using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.OnlineResources;
using Unify.Domain.UniversityCore;

namespace Unify.Infrastructure.Configurations.OnlineResources;

internal sealed class CourseResourceConfiguration : OnlineResourceConfiguration<CourseResource>
{
    public override void Configure(EntityTypeBuilder<CourseResource> builder)
    {
        builder.ToTable("course_resources");

        builder.HasOne(cr => cr.Course)
            .WithMany()
            .HasForeignKey("course_id");

        builder.Property<Guid>("course_id").IsRequired();

        base.Configure(builder);
    }
}