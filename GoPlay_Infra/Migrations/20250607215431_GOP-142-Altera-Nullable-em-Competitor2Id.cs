using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoPlay_Infra.Migrations
{
    /// <inheritdoc />
    public partial class GOP142AlteraNullableemCompetitor2Id : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Competitor2Id",
                table: "GameMatch",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Competitor2Id",
                table: "GameMatch",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
