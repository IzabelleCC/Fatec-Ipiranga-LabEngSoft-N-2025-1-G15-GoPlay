using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoPlay_Infra.Migrations
{
    /// <inheritdoc />
    public partial class GOP142AdicionaCategoryIdemGameMatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "GameMatch",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_GameMatch_CategoryId",
                table: "GameMatch",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameMatch_Category_CategoryId",
                table: "GameMatch",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameMatch_Category_CategoryId",
                table: "GameMatch");

            migrationBuilder.DropIndex(
                name: "IX_GameMatch_CategoryId",
                table: "GameMatch");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "GameMatch");
        }
    }
}
