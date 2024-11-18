using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.OnlineResources;
using Unify.Domain.Shared;
using Unify.Domain.UniversityClasses;

namespace Unify.Infrastructure.Configurations.OnlineResources;

internal sealed class HomeworkAssigmentConfiguration : IEntityTypeConfiguration<HomeworkAssigment>
{
    public void Configure(EntityTypeBuilder<HomeworkAssigment> builder)
    {
        builder.ToTable("homework_assigments");

        builder.HasKey(ha => ha.Id);

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

        builder.Property(ha => ha.Mark);

        builder.Property(ha => ha.Feedback)
            .HasMaxLength(200)
            .HasConversion(feedback => feedback == null ? "" : feedback.Value, value => new TextContent(value));

        builder.HasMany(ha => ha.Files)
            .WithOne()
            .HasForeignKey("homework_assigment_id");

        builder.HasMany(ha => ha.Submissions)
            .WithOne()
            .HasForeignKey(hs => hs.HomeworkAssigment);

        builder.HasOne<ClassOffering>()
            .WithMany()
            .HasForeignKey(ha => ha.ClassOffering);
    }
}
