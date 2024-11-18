using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.Messages;

namespace Unify.Infrastructure.Configurations;

internal sealed class MessageAttachmentsConfiguration : IEntityTypeConfiguration<MessageAttachments>
{
    public void Configure(EntityTypeBuilder<MessageAttachments> builder)
    {
        builder.ToTable("message_attachments");

        builder.HasKey(messageAttachment =>
            new { messageAttachment.MessageId, Attachment = messageAttachment.AttachmentId });
    }
}