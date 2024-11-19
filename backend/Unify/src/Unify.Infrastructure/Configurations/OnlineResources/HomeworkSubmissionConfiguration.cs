using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.OnlineResources;
using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;
using Unify.Domain.Users;

namespace Unify.Infrastructure.Configurations.OnlineResources;

internal sealed class HomeworkSubmissionConfiguration : HomeworkBaseEntityConfiguration<HomeworkSubmission>
{
    public void Configure(EntityTypeBuilder<HomeworkSubmission> builder)
    {
        builder.ToTable("homework_submissions");

        builder.Property(hs => hs.SubmittedOn)
            .IsRequired();

        builder.Property(ha => ha.Feedback)
            .HasMaxLength(200)
            .HasConversion(feedback => feedback == null ? "" : feedback.Value, value => new TextContent(value));

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(hs => hs.StudentId);

        builder.HasOne<Mark>()
            .WithMany()
            .HasForeignKey(hs => hs.MarkId);

        builder.HasOne<HomeworkAssigment>()
            .WithMany()
            .HasForeignKey(hs => hs.HomeworkAssigmentId);
    }
}