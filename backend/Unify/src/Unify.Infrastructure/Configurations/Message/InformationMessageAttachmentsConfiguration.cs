using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.Messages.InformationMessages;

namespace Unify.Infrastructure.Configurations;

internal sealed class InformationMessageAttachmentsConfiguration : IEntityTypeConfiguration<InformationMessageAttachments>
{
    public void Configure(EntityTypeBuilder<InformationMessageAttachments> builder)
    {
        builder.ToTable("information_message_attachments");

        builder.HasKey(messageAttachments => new { messageAttachments.InformationMessageId, messageAttachments.AttachmentId});
    }
}