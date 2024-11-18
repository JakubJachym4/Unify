using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;

namespace Unify.Infrastructure.Configurations.UniversityCore;

internal sealed class MarkConfiguration : IEntityTypeConfiguration<Mark>
{
    public void Configure(EntityTypeBuilder<Mark> builder)
    {
        builder.ToTable("marks");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Score)
            .IsRequired()
            .HasConversion(s => s.Value, value => value);

        builder.Property(m => m.MaxScore)
            .IsRequired()
            .HasConversion(s => s.Value, value => value);

        builder.Property(m => m.Criteria)
            .HasMaxLength(400)
            .HasConversion(d => d.Value, value => new Description(value));;

    }
}