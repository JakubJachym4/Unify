using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.UniversityClasses;
using Unify.Domain.Users;

namespace Unify.Infrastructure.Configurations.UniversityClasses;

internal sealed class ClassEnrollmentConfiguration : IEntityTypeConfiguration<ClassEnrollment>
{
    public void Configure(EntityTypeBuilder<ClassEnrollment> builder)
    {
        builder.ToTable("class_enrollments");
        builder.HasKey(ce => ce.Id);

        builder.Property(ce => ce.EnrollmentOn)
            .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(ce => ce.Student);

        builder.HasMany(ce => ce.Grades)
            .WithOne()
            .HasForeignKey("class_enrollment_id");

        builder.HasOne<ClassOffering>()
            .WithMany()
            .HasForeignKey(ce => ce.ClassOffering);
    }
}