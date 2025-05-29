using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoPlay_Infra.Migrations
{
    /// <inheritdoc />
    public partial class GOP104Adicionacamposreferentestatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FirstUserPaymentConfirmed",
                table: "CategoryPlayer",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RegisterStatus",
                table: "CategoryPlayer",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SecondUserPaymentConfirmed",
                table: "CategoryPlayer",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstUserPaymentConfirmed",
                table: "CategoryPlayer");

            migrationBuilder.DropColumn(
                name: "RegisterStatus",
                table: "CategoryPlayer");

            migrationBuilder.DropColumn(
                name: "SecondUserPaymentConfirmed",
                table: "CategoryPlayer");
        }
    }
}
