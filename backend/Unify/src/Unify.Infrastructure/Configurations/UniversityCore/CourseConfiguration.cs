using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;

namespace Unify.Infrastructure.Configurations.UniversityCore;

internal sealed class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("courses");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasConversion(n => n.Value, value => new Name(value));

        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(400)
            .HasConversion(d => d.Value, value => new Description(value));;

        builder.HasOne<Specialization>()
            .WithMany()
            .HasForeignKey(c => c.Specialization);

        builder.HasMany(c => c.Classes)
            .WithOne()
            .HasForeignKey(co => co.Course);

    }
}