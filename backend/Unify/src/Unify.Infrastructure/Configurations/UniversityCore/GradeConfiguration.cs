using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.Shared;
using Unify.Domain.UniversityClasses;
using Unify.Domain.UniversityCore;

namespace Unify.Infrastructure.Configurations.UniversityCore;

internal sealed class GradeConfiguration : IEntityTypeConfiguration<Grade>
{
    public void Configure(EntityTypeBuilder<Grade> builder)
    {
        builder.ToTable("grade");

        builder.HasKey(g => g.Id);
        builder.Property(g => g.Description)
            .IsRequired()
            .HasMaxLength(400)
            .HasConversion(d => d.Value, value => new Description(value));;

        builder.Property(g => g.Score)
            .HasConversion(s => s == null ? (decimal?)null : s.Value, value => value == null ? null : new Score(value.Value));

        builder.Property(g => g.DateAwarded);

        builder.HasMany(g => g.Marks)
            .WithOne()
            .HasForeignKey(mark => mark.GradeId);
    }
}