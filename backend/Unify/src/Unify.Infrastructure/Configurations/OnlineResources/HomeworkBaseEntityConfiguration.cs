using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.OnlineResources;

namespace Unify.Infrastructure.Configurations.OnlineResources;

internal abstract class HomeworkBaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : HomeworkBaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasMany<Attachment>()
            .WithMany()
            .UsingEntity<HomeworkBasesAttachments>();
    }
}