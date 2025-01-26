using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class another_one_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_attachments_course_resources_online_resource_id",
                table: "attachments");

            migrationBuilder.DropForeignKey(
                name: "fk_attachments_offering_resources_online_resource_id",
                table: "attachments");

            migrationBuilder.RenameColumn(
                name: "online_resource_id",
                table: "attachments",
                newName: "offering_resources_id");

            migrationBuilder.RenameIndex(
                name: "ix_attachments_online_resource_id",
                table: "attachments",
                newName: "ix_attachments_offering_resources_id");

            migrationBuilder.AddColumn<Guid>(
                name: "course_resources_id",
                table: "attachments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_attachments_course_resources_id",
                table: "attachments",
                column: "course_resources_id");

            migrationBuilder.AddForeignKey(
                name: "fk_attachments_course_resources_course_resources_id",
                table: "attachments",
                column: "course_resources_id",
                principalTable: "course_resources",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_attachments_offering_resources_offering_resources_id",
                table: "attachments",
                column: "offering_resources_id",
                principalTable: "offering_resources",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_attachments_course_resources_course_resources_id",
                table: "attachments");

            migrationBuilder.DropForeignKey(
                name: "fk_attachments_offering_resources_offering_resources_id",
                table: "attachments");

            migrationBuilder.DropIndex(
                name: "ix_attachments_course_resources_id",
                table: "attachments");

            migrationBuilder.DropColumn(
                name: "course_resources_id",
                table: "attachments");

            migrationBuilder.RenameColumn(
                name: "offering_resources_id",
                table: "attachments",
                newName: "online_resource_id");

            migrationBuilder.RenameIndex(
                name: "ix_attachments_offering_resources_id",
                table: "attachments",
                newName: "ix_attachments_online_resource_id");

            migrationBuilder.AddForeignKey(
                name: "fk_attachments_course_resources_online_resource_id",
                table: "attachments",
                column: "online_resource_id",
                principalTable: "course_resources",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_attachments_offering_resources_online_resource_id",
                table: "attachments",
                column: "online_resource_id",
                principalTable: "offering_resources",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
