using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoPlay_Infra.Migrations
{
    /// <inheritdoc />
    public partial class GOP142ExcluiGroupNumberemGameMatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupNumber",
                table: "GameMatch");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupNumber",
                table: "GameMatch",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
