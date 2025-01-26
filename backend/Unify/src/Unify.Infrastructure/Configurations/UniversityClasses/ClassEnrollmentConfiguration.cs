using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.UniversityClasses;
using Unify.Domain.UniversityCore;
using Unify.Domain.Users;

namespace Unify.Infrastructure.Configurations.UniversityClasses;

internal sealed class ClassEnrollmentConfiguration : IEntityTypeConfiguration<ClassEnrollment>
{
    public void Configure(EntityTypeBuilder<ClassEnrollment> builder)
    {
        builder.ToTable("class_enrollments");
        builder.HasKey(ce => ce.Id);

        builder.Property(ce => ce.EnrolledOn)
            .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(ce => ce.StudentId);

        builder.HasOne<ClassOffering>()
            .WithMany()
            .HasForeignKey(ce => ce.ClassOfferingId);

        builder.HasOne<Grade>()
            .WithMany()
            .HasForeignKey(ce => ce.GradeId);
    }
}