using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unify.Domain.UniversityCore;

namespace Unify.Infrastructure.Configurations.UniversityCore;

internal sealed class StudentGroupsUsersConfiguration : IEntityTypeConfiguration<StudentGroupsUsers>
{
    public void Configure(EntityTypeBuilder<StudentGroupsUsers> builder)
    {
        builder.ToTable("student_groups_users");
        builder.HasKey(x => new { x.StudentGroupId, x.StudentId });
    }
}