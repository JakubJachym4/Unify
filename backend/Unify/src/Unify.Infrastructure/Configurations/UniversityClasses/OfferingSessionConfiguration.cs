using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.UniversityClasses;

namespace Unify.Infrastructure.Configurations.UniversityClasses;

internal sealed class OfferingSessionConfiguration : ClassSessionConfiguration<OfferingSession>
{
    public override void Configure(EntityTypeBuilder<OfferingSession> builder)
    {
        builder.ToTable("offering_sessions");

        builder.HasOne(os => os.ClassOffering)
            .WithMany()
            .HasForeignKey("class_offering_id");

        base.Configure(builder);
    }
}