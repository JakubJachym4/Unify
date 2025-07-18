﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class markDateAwarded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "date_awarded",
                table: "marks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "date_awarded",
                table: "marks");
        }
    }
}
