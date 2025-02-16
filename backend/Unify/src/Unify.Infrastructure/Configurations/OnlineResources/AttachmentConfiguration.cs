using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.OnlineResources;

namespace Unify.Infrastructure.Configurations.OnlineResources;

internal sealed class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.ToTable("attachments");

        builder.HasKey(attachment => attachment.Id);

        builder.Property(attachment =>  attachment.FileName).IsRequired();
        builder.Property(attachment => attachment.Data).IsRequired();

        builder.HasMany<HomeworkAssignment>()
            .WithOne()
            .HasForeignKey("homework_assignment_id").OnDelete(DeleteBehavior.Cascade);
    }
}