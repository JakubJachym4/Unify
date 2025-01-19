using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;

namespace Unify.Infrastructure.Configurations.UniversityCore;

internal sealed class SpecializationConfiguration : IEntityTypeConfiguration<Specialization>
{
    public void Configure(EntityTypeBuilder<Specialization> builder)
    {
        builder.ToTable("specializations");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasConversion(s => s.Value, value => new Name(value));

        builder.Property(s => s.Description)
            .IsRequired()
            .HasMaxLength(800)
            .HasConversion(d => d.Value, value => new Description(value));

        builder.HasOne<FieldOfStudy>()
            .WithMany()
            .HasForeignKey(s => s.FieldOfStudyId);

        builder.HasMany(s => s.Students)
            .WithOne()
            .HasForeignKey(u => u.SpecializationId);
    }
}