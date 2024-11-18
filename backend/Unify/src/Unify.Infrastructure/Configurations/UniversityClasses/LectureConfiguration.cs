using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.UniversityClasses;
using Unify.Domain.UniversityCore;

namespace Unify.Infrastructure.Configurations.UniversityClasses;

internal sealed class LectureConfiguration : ClassSessionConfiguration<Lecture>
{
    public override void Configure(EntityTypeBuilder<Lecture> builder)
    {
        builder.ToTable("lectures");

        builder.HasOne<Course>()
            .WithMany()
            .HasForeignKey(l => l.Course);

        base.Configure(builder);

    }
}