using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;

namespace Unify.Infrastructure.Configurations.UniversityCore;

internal sealed class FieldOfStudyConfiguration : IEntityTypeConfiguration<FieldOfStudy>
{
    public void Configure(EntityTypeBuilder<FieldOfStudy> builder)
    {
        builder.ToTable("fields_of_study");
        builder.HasKey(x => x.Id);

        builder.Property(fos => fos.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasConversion(fos => fos.Value, value => new Name(value));

        builder.Property(fos => fos.Description)
            .IsRequired()
            .HasMaxLength(800)
            .HasConversion(fos => fos.Value, value => new Description(value));

        builder.HasOne<Faculty>()
            .WithOne()
            .HasForeignKey<FieldOfStudy>(fos => fos.Faculty);

        builder.HasMany(fos => fos.Specializations)
            .WithOne()
            .HasForeignKey(s => s.FieldOfStudy);
    }
}