using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GoPlay_Infra.Migrations
{
    /// <inheritdoc />
    public partial class GOP104AdicionanovoscamposnatabelaCategoryPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryPlayer_AspNetUsers_UserId",
                table: "CategoryPlayer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryPlayer",
                table: "CategoryPlayer");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "CategoryPlayer",
                newName: "FirstUserId");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryPlayer_UserId",
                table: "CategoryPlayer",
                newName: "IX_CategoryPlayer_FirstUserId");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "CategoryPlayer",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "SecondUserId",
                table: "CategoryPlayer",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserEntityId",
                table: "Category",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryPlayer",
                table: "CategoryPlayer",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryPlayer_CategoryId",
                table: "CategoryPlayer",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryPlayer_SecondUserId",
                table: "CategoryPlayer",
                column: "SecondUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_UserEntityId",
                table: "Category",
                column: "UserEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Category_AspNetUsers_UserEntityId",
                table: "Category",
                column: "UserEntityId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryPlayer_AspNetUsers_FirstUserId",
                table: "CategoryPlayer",
                column: "FirstUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryPlayer_AspNetUsers_SecondUserId",
                table: "CategoryPlayer",
                column: "SecondUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_AspNetUsers_UserEntityId",
                table: "Category");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryPlayer_AspNetUsers_FirstUserId",
                table: "CategoryPlayer");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryPlayer_AspNetUsers_SecondUserId",
                table: "CategoryPlayer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryPlayer",
                table: "CategoryPlayer");

            migrationBuilder.DropIndex(
                name: "IX_CategoryPlayer_CategoryId",
                table: "CategoryPlayer");

            migrationBuilder.DropIndex(
                name: "IX_CategoryPlayer_SecondUserId",
                table: "CategoryPlayer");

            migrationBuilder.DropIndex(
                name: "IX_Category_UserEntityId",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CategoryPlayer");

            migrationBuilder.DropColumn(
                name: "SecondUserId",
                table: "CategoryPlayer");

            migrationBuilder.DropColumn(
                name: "UserEntityId",
                table: "Category");

            migrationBuilder.RenameColumn(
                name: "FirstUserId",
                table: "CategoryPlayer",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryPlayer_FirstUserId",
                table: "CategoryPlayer",
                newName: "IX_CategoryPlayer_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryPlayer",
                table: "CategoryPlayer",
                columns: new[] { "CategoryId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryPlayer_AspNetUsers_UserId",
                table: "CategoryPlayer",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
