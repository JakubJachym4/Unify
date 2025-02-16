using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class cascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "homework_assignment_id",
                table: "homework_assignments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_homework_assignments_homework_assignment_id",
                table: "homework_assignments",
                column: "homework_assignment_id");

            migrationBuilder.AddForeignKey(
                name: "fk_homework_assignments_attachments_homework_assignment_id",
                table: "homework_assignments",
                column: "homework_assignment_id",
                principalTable: "attachments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_homework_assignments_attachments_homework_assignment_id",
                table: "homework_assignments");

            migrationBuilder.DropIndex(
                name: "ix_homework_assignments_homework_assignment_id",
                table: "homework_assignments");

            migrationBuilder.DropColumn(
                name: "homework_assignment_id",
                table: "homework_assignments");
        }
    }
}
