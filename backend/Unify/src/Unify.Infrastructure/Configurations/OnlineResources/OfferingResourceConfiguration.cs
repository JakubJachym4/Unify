using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.OnlineResources;
using Unify.Domain.UniversityClasses;

namespace Unify.Infrastructure.Configurations.OnlineResources;

internal sealed class OfferingResourceConfiguration : OnlineResourceConfiguration<OfferingResource>
{
    public override void Configure(EntityTypeBuilder<OfferingResource> builder)
    {
        builder.ToTable("offering_resources");

        builder.HasOne<ClassOffering>()
            .WithMany()
            .HasForeignKey(or => or.ClassOffering);

        base.Configure(builder);
    }
}