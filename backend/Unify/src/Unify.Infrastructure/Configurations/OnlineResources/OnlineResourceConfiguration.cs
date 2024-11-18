using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.OnlineResources;
using Unify.Domain.Shared;

namespace Unify.Infrastructure.Configurations.OnlineResources;

internal abstract class OnlineResourceConfiguration<T> : IEntityTypeConfiguration<T> where T : OnlineResource
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(or => or.Id);

        builder.Property(or => or.Title)
            .IsRequired()
            .HasMaxLength(60)
            .HasConversion(title => title.Value, value => new Title(value));

        builder.Property(or => or.Description)
            .IsRequired()
            .HasMaxLength(200)
            .HasConversion(description => description.Value, value => new Description(value));

        builder.HasMany(or => or.Files)
            .WithOne()
            .HasForeignKey("online_resource_id");

    }
}