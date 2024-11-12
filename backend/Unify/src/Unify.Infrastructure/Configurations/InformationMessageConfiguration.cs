using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.Messages;
using Unify.Domain.Messages.InformationMessages;
using Unify.Domain.Users;

namespace Unify.Infrastructure.Configurations;

internal sealed class InformationMessageConfiguration : IEntityTypeConfiguration<InformationMessage>
{
    public void Configure(EntityTypeBuilder<InformationMessage> builder)
    {
        builder.ToTable("information_messages");

        builder.HasKey(message => message.Id);

        builder.Property(message => message.Title)
            .IsRequired()
            .HasMaxLength(400)
            .HasConversion(title => title.Value, value => new Title(value));

        builder.Property(message => message.Content)
            .IsRequired()
            .HasConversion(content => content.Value, value => new TextContent(value));

        builder.Property(message => message.ExpirationDate)
            .IsRequired();

        builder.Property(message => message.CreatedOn)
            .IsRequired();

        builder.Property(message => message.SeverityLevel)
            .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(message => message.SenderId);

        builder.HasMany(message => message.Recipients)
            .WithMany()
            .UsingEntity<InformationMessageUser>();

        builder.HasMany(message => message.Attachments)
            .WithMany()
            .UsingEntity<InformationMessageAttachments>();
    }
}