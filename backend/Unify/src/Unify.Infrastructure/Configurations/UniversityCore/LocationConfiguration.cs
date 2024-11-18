using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.UniversityCore;

namespace Unify.Infrastructure.Configurations.UniversityCore;

internal sealed class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Building)
            .HasMaxLength(80);

      builder.Property(l => l.Street)
            .HasMaxLength(200);

      builder.Property(l => l.DoorNumber)
            .HasMaxLength(20);

      builder.Property(l => l.Floor);

      builder.Property(l => l.Building)
            .HasMaxLength(200);

      builder.Property(l => l.MeetingUrl)
          .HasMaxLength(400);

        builder.Property(l => l.Online)
            .IsRequired();

        builder.HasOne<Faculty>()
            .WithMany()
            .HasForeignKey(l => l.Faculty);
    }
}