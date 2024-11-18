using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.Messages;

namespace Unify.Infrastructure.Configurations;

internal sealed class MessageUserConfiguration : IEntityTypeConfiguration<MessageUser>
{
    public void Configure(EntityTypeBuilder<MessageUser> builder)
    {
        builder.ToTable("messages_users");

        builder.HasKey(message => new { message.MessageId, message.UserId });

    }
}