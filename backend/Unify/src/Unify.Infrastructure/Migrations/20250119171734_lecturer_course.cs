using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class lecturer_course : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "enrollment_on",
                table: "class_enrollments",
                newName: "enrolled_on");

            migrationBuilder.AddColumn<Guid>(
                name: "lecturer_id",
                table: "courses",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_courses_lecturer_id",
                table: "courses",
                column: "lecturer_id");

            migrationBuilder.AddForeignKey(
                name: "fk_courses_user_lecturer_id",
                table: "courses",
                column: "lecturer_id",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_courses_user_lecturer_id",
                table: "courses");

            migrationBuilder.DropIndex(
                name: "ix_courses_lecturer_id",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "lecturer_id",
                table: "courses");

            migrationBuilder.RenameColumn(
                name: "enrolled_on",
                table: "class_enrollments",
                newName: "enrollment_on");
        }
    }
}
