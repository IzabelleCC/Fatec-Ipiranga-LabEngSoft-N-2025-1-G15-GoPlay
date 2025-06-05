using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoPlay_Infra.Migrations
{
    /// <inheritdoc />
    public partial class GOP142Adiciona_camposnovosnatabelaMatchGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MatchStage",
                table: "MatchGroup");

            migrationBuilder.DropColumn(
                name: "Result",
                table: "MatchGroup");

            migrationBuilder.AddColumn<int>(
                name: "Games",
                table: "MatchGroup",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Losses",
                table: "MatchGroup",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "MatchGroup",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sets",
                table: "MatchGroup",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Tiebreaks",
                table: "MatchGroup",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Wins",
                table: "MatchGroup",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Games",
                table: "MatchGroup");

            migrationBuilder.DropColumn(
                name: "Losses",
                table: "MatchGroup");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "MatchGroup");

            migrationBuilder.DropColumn(
                name: "Sets",
                table: "MatchGroup");

            migrationBuilder.DropColumn(
                name: "Tiebreaks",
                table: "MatchGroup");

            migrationBuilder.DropColumn(
                name: "Wins",
                table: "MatchGroup");

            migrationBuilder.AddColumn<int>(
                name: "MatchStage",
                table: "MatchGroup",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Result",
                table: "MatchGroup",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
