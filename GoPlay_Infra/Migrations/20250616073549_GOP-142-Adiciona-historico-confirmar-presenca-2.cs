using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoPlay_Infra.Migrations
{
    /// <inheritdoc />
    public partial class GOP142Adicionahistoricoconfirmarpresenca2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttendanceConfirmedUserId",
                table: "MatchGroup",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AttendanceTime",
                table: "MatchGroup",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttendanceConfirmedUserId",
                table: "MatchGroup");

            migrationBuilder.DropColumn(
                name: "AttendanceTime",
                table: "MatchGroup");
        }
    }
}
