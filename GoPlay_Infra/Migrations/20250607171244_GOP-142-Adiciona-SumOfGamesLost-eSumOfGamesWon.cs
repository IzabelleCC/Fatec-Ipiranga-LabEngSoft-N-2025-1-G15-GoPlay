using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoPlay_Infra.Migrations
{
    /// <inheritdoc />
    public partial class GOP142AdicionaSumOfGamesLosteSumOfGamesWon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SumOfGames",
                table: "MatchGroup",
                newName: "SumOfGamesWon");

            migrationBuilder.AddColumn<int>(
                name: "SumOfGamesLost",
                table: "MatchGroup",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SumOfGamesLost",
                table: "MatchGroup");

            migrationBuilder.RenameColumn(
                name: "SumOfGamesWon",
                table: "MatchGroup",
                newName: "SumOfGames");
        }
    }
}
