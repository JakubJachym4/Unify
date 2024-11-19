using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;

namespace Unify.Infrastructure.Configurations.UniversityCore;

internal sealed class FacultyConfiguration : IEntityTypeConfiguration<Faculty>
{
    public void Configure(EntityTypeBuilder<Faculty> builder)
    {
        builder.ToTable("faculties");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50)
            .HasConversion(name => name.Value, value => new Name(value));
    }
}