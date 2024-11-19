using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.Shared;
using Unify.Domain.UniversityClasses;
using Unify.Domain.UniversityCore;
using Unify.Domain.Users;

namespace Unify.Infrastructure.Configurations.UniversityClasses;

internal abstract class ClassSessionConfiguration<T> : IEntityTypeConfiguration<T> where T : ClassSession
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(cs => cs.Id);

        builder.Property(cs => cs.Title)
            .IsRequired()
            .HasMaxLength(100)
            .HasConversion(title => title.Value, value => new Title(value));


        builder.Property(cs => cs.ClassType)
            .IsRequired();

        builder.Property(cs => cs.ScheduledDate)
            .IsRequired();

        builder.Property(cs => cs.Duration)
            .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(cs => cs.LecturerId);

        builder.HasOne<Location>()
            .WithMany()
            .HasForeignKey(cs => cs.LocationId);
    }
}