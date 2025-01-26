using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class userindex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_users_specialization_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_student_group_id",
                table: "users");

            migrationBuilder.CreateIndex(
                name: "ix_users_specialization_id",
                table: "users",
                column: "specialization_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_student_group_id",
                table: "users",
                column: "student_group_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_users_specialization_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_student_group_id",
                table: "users");

            migrationBuilder.CreateIndex(
                name: "ix_users_specialization_id",
                table: "users",
                column: "specialization_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_student_group_id",
                table: "users",
                column: "student_group_id",
                unique: true);
        }
    }
}
