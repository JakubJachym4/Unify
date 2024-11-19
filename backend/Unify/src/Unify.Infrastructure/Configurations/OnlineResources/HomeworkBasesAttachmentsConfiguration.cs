using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.OnlineResources;

namespace Unify.Infrastructure.Configurations.OnlineResources;

internal sealed class HomeworkBasesAttachmentsConfiguration : IEntityTypeConfiguration<HomeworkBasesAttachments>
{
    public void Configure(EntityTypeBuilder<HomeworkBasesAttachments> builder)
    {
        builder.ToTable("homework_bases_attachments");
        builder.HasKey(x => new { x.AttachmentId, x.HomeworkBaseId });
    }
}