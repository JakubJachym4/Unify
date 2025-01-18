using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;

namespace Unify.Infrastructure.Configurations.UniversityCore;

internal sealed class StudentGroupConfiguration : IEntityTypeConfiguration<StudentGroup>
{
    public void Configure(EntityTypeBuilder<StudentGroup> builder)
    {
        builder.ToTable("student_group");
        builder.HasKey(sg => sg.Id);

        builder.Property(sg => sg.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasConversion(n => n.Value, value => new Name(value));

        builder.Property(sg => sg.StudyYear)
            .IsRequired()
            .HasConversion(sy => sy.StartingYear, value => new StudyYear(value));

        builder.Property(sg => sg.Semester)
            .IsRequired()
            .HasConversion(semester => semester.Value, value => new Semester(value));

        builder.Property(sg => sg.Term)
            .IsRequired();

        builder.Property(sg => sg.MaxGroupSize)
            .IsRequired();

        builder.HasOne<Specialization>()
            .WithMany()
            .HasForeignKey(sg => sg.SpecializationId);

        builder.HasMany(sg => sg.Members)
            .WithOne()
            .HasForeignKey("student_group_id");

        builder.HasMany(sg => sg.ClassEnrollments)
            .WithOne()
            .HasForeignKey(ce => ce.StudentGroupId);
    }
}