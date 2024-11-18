using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.OnlineResources;
using Unify.Domain.Users;

namespace Unify.Infrastructure.Configurations.OnlineResources;

internal sealed class HomeworkSubmissionConfiguration : IEntityTypeConfiguration<HomeworkSubmission>
{
    public void Configure(EntityTypeBuilder<HomeworkSubmission> builder)
    {
        builder.ToTable("homework_submissions");

        builder.HasKey(hs => hs.Id);

        builder.Property(hs => hs.SubmittedOn)
            .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(hs => hs.Student);

        builder.HasMany(hs => hs.Files)
            .WithOne()
            .HasForeignKey("homework_submission_id");
    }
}