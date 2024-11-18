using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.Shared;
using Unify.Domain.UniversityClasses;
using Unify.Domain.UniversityCore;
using Unify.Domain.Users;

namespace Unify.Infrastructure.Configurations.UniversityClasses;

internal sealed class ClassOfferingConfiguration : IEntityTypeConfiguration<ClassOffering>
{
    public void Configure(EntityTypeBuilder<ClassOffering> builder)
    {
        builder.ToTable("class_offerings");
        builder.HasKey(co => co.Id);

        builder.Property(co => co.MaxStudentsCount)
            .IsRequired();

        builder.Property(co => co.Name)
            .IsRequired()
            .HasMaxLength(60)
            .HasConversion(name => name.Value, value => new Name(value));

        builder.Property(co => co.StartDate)
            .IsRequired();

        builder.Property(co => co.EndDate)
            .IsRequired();

        builder.HasOne<Course>()
            .WithMany()
            .HasForeignKey(co => co.Course);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(co => co.Lecturer);

        builder.HasOne<StudentGroup>()
            .WithMany()
            .HasForeignKey(co => co.BoundGroup);

        builder.HasMany(co => co.Enrollments)
            .WithOne()
            .HasForeignKey(ce => ce.ClassOffering);

        builder.HasMany(co => co.Messages)
            .WithOne()
            .HasForeignKey("class_offering_id");
    }
}