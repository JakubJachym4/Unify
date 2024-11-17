using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.Messages;
using Unify.Domain.Shared;
using Unify.Domain.Users;

namespace Unify.Infrastructure.Configurations;

internal sealed class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("messages");

        builder.HasKey(message => message.Id);

        builder.Property(message => message.Title)
            .IsRequired()
            .HasMaxLength(400)
            .HasConversion(title => title.Value, value => new Title(value));

        builder.Property(message => message.Content)
            .IsRequired()
            .HasConversion(content => content.Value, value => new TextContent(value));

        builder.Property(message => message.Status)
            .IsRequired();

        builder.Property(message => message.CreatedOn)
            .IsRequired();

        builder.Property(message => message.SenderId)
            .IsRequired()
            .HasColumnName("sender_id");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(message => message.SenderId);

        builder.HasMany(s => s.Recipients)
            .WithMany()
            .UsingEntity<MessageUser>();

        builder.HasMany(message => message.Attachments)
            .WithMany()
            .UsingEntity<MessageAttachments>();
    }
}