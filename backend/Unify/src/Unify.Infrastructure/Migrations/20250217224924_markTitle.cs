using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class markTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "criteria",
                table: "marks");

            migrationBuilder.AddColumn<string>(
                name: "title",
                table: "marks",
                type: "character varying(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "title",
                table: "marks");

            migrationBuilder.AddColumn<string>(
                name: "criteria",
                table: "marks",
                type: "character varying(400)",
                maxLength: 400,
                nullable: true);
        }
    }
}
