using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.UniversityClasses;

namespace Unify.Infrastructure.Configurations.UniversityClasses;

internal sealed class ClassOfferingSessionConfiguration : ClassSessionConfiguration<ClassOfferingSession>
{
    public override void Configure(EntityTypeBuilder<ClassOfferingSession> builder)
    {
        builder.ToTable("class_offering_sessions");

        builder.HasOne<ClassOffering>()
            .WithMany()
            .HasForeignKey(os => os.ClassOfferingId);

        base.Configure(builder);
    }
}