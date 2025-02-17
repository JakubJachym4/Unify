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

        builder.Property(m => m.Title)
            .IsRequired()
            .HasMaxLength(60)
            .HasConversion(t => t.Value, value => new Title(value));;

        builder.Property(m => m.DateAwarded)
            .IsRequired();

    }
}