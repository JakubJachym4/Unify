using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.Messages.InformationMessages;

namespace Unify.Infrastructure.Configurations;

internal sealed class InformationMessageUserConfiguration : IEntityTypeConfiguration<InformationMessageUser>
{
    public void Configure(EntityTypeBuilder<InformationMessageUser> builder)
    {
        builder.ToTable("information_message_user");

        builder.HasKey(messageUser =>
            new { messageUser.InformationMessageId, messageUser.UserId });
    }
}