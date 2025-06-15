using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoPlay_Infra.Migrations
{
    /// <inheritdoc />
    public partial class GOP142Adicionanovascolunasgames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Game10",
                table: "MatchGroup",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Game6",
                table: "MatchGroup",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Game7",
                table: "MatchGroup",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Game8",
                table: "MatchGroup",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Game9",
                table: "MatchGroup",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Game10",
                table: "MatchGroup");

            migrationBuilder.DropColumn(
                name: "Game6",
                table: "MatchGroup");

            migrationBuilder.DropColumn(
                name: "Game7",
                table: "MatchGroup");

            migrationBuilder.DropColumn(
                name: "Game8",
                table: "MatchGroup");

            migrationBuilder.DropColumn(
                name: "Game9",
                table: "MatchGroup");
        }
    }
}
