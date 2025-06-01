using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoPlay_Infra.Migrations
{
    /// <inheritdoc />
    public partial class GOP105AdicionaFirstUserTxIdeSecondUserTxId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TxId",
                table: "CategoryPlayer",
                newName: "SecondUserTxId");

            migrationBuilder.AddColumn<string>(
                name: "FirstUserTxId",
                table: "CategoryPlayer",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstUserTxId",
                table: "CategoryPlayer");

            migrationBuilder.RenameColumn(
                name: "SecondUserTxId",
                table: "CategoryPlayer",
                newName: "TxId");
        }
    }
}
