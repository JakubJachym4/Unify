using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class another_one : Migration
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

            migrationBuilder.DropForeignKey(
                name: "fk_grade_class_enrollments_class_enrollment_id",
                table: "grade");

            migrationBuilder.DropIndex(
                name: "ix_grade_class_enrollment_id",
                table: "grade");

            migrationBuilder.DropColumn(
                name: "class_enrollment_id",
                table: "grade");

            migrationBuilder.AddColumn<Guid>(
                name: "grade_id",
                table: "class_enrollments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_class_enrollments_grade_id",
                table: "class_enrollments",
                column: "grade_id");

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

            migrationBuilder.AddForeignKey(
                name: "fk_class_enrollments_grade_grade_id",
                table: "class_enrollments",
                column: "grade_id",
                principalTable: "grade",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_attachments_course_resources_online_resource_id",
                table: "attachments");

            migrationBuilder.DropForeignKey(
                name: "fk_attachments_offering_resources_online_resource_id",
                table: "attachments");

            migrationBuilder.DropForeignKey(
                name: "fk_class_enrollments_grade_grade_id",
                table: "class_enrollments");

            migrationBuilder.DropIndex(
                name: "ix_class_enrollments_grade_id",
                table: "class_enrollments");

            migrationBuilder.DropColumn(
                name: "grade_id",
                table: "class_enrollments");

            migrationBuilder.AddColumn<Guid>(
                name: "class_enrollment_id",
                table: "grade",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_grade_class_enrollment_id",
                table: "grade",
                column: "class_enrollment_id");

            migrationBuilder.AddForeignKey(
                name: "fk_attachments_course_resources_online_resource_id",
                table: "attachments",
                column: "online_resource_id",
                principalTable: "course_resources",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_attachments_offering_resources_online_resource_id",
                table: "attachments",
                column: "online_resource_id",
                principalTable: "offering_resources",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_grade_class_enrollments_class_enrollment_id",
                table: "grade",
                column: "class_enrollment_id",
                principalTable: "class_enrollments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
