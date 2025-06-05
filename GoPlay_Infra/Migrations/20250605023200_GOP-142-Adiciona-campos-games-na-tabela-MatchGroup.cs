using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoPlay_Infra.Migrations
{
    /// <inheritdoc />
    public partial class GOP142AdicionacamposgamesnatabelaMatchGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Sets",
                table: "MatchGroup",
                newName: "SetsBalance");

            migrationBuilder.RenameColumn(
                name: "Games",
                table: "MatchGroup",
                newName: "GamesBalance");

            migrationBuilder.AddColumn<int>(
                name: "Game1",
                table: "MatchGroup",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Game2",
                table: "MatchGroup",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Game3",
                table: "MatchGroup",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Game4",
                table: "MatchGroup",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Game5",
                table: "MatchGroup",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Game1",
                table: "MatchGroup");

            migrationBuilder.DropColumn(
                name: "Game2",
                table: "MatchGroup");

            migrationBuilder.DropColumn(
                name: "Game3",
                table: "MatchGroup");

            migrationBuilder.DropColumn(
                name: "Game4",
                table: "MatchGroup");

            migrationBuilder.DropColumn(
                name: "Game5",
                table: "MatchGroup");

            migrationBuilder.RenameColumn(
                name: "SetsBalance",
                table: "MatchGroup",
                newName: "Sets");

            migrationBuilder.RenameColumn(
                name: "GamesBalance",
                table: "MatchGroup",
                newName: "Games");
        }
    }
}
