using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoPlay_Infra.Migrations
{
    /// <inheritdoc />
    public partial class GOP211AdicionavinculoTournamenteUserAdm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdmUserId",
                table: "Tournament",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Tournament_AdmUserId",
                table: "Tournament",
                column: "AdmUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tournament_AspNetUsers_AdmUserId",
                table: "Tournament",
                column: "AdmUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tournament_AspNetUsers_AdmUserId",
                table: "Tournament");

            migrationBuilder.DropIndex(
                name: "IX_Tournament_AdmUserId",
                table: "Tournament");

            migrationBuilder.DropColumn(
                name: "AdmUserId",
                table: "Tournament");
        }
    }
}
