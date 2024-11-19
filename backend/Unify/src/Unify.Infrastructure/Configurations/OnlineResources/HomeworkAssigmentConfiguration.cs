using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.OnlineResources;
using Unify.Domain.Shared;
using Unify.Domain.UniversityClasses;

namespace Unify.Infrastructure.Configurations.OnlineResources;

internal sealed class HomeworkAssigmentConfiguration : HomeworkBaseEntityConfiguration<HomeworkAssigment>
{
    public void Configure(EntityTypeBuilder<HomeworkAssigment> builder)
    {
        builder.ToTable("homework_assignments");

        builder.Property(ha => ha.Title)
            .IsRequired()
            .HasMaxLength(60)
            .HasConversion(title => title.Value, value => new Title(value));

        builder.Property(ha => ha.Description)
            .IsRequired()
            .HasMaxLength(200)
            .HasConversion(description => description.Value, value => new Description(value));

        builder.Property(ha => ha.DueDate)
            .IsRequired();

        builder.Property(ha => ha.Locked)
            .IsRequired();

        builder.HasMany(ha => ha.Submissions)
            .WithOne()
            .HasForeignKey(hs => hs.HomeworkAssigmentId);

        builder.HasOne<ClassOffering>()
            .WithMany()
            .HasForeignKey(ha => ha.ClassOfferingId);
    }
}
