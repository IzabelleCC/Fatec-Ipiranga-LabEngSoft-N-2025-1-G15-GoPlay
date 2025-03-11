using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoPlay_UserManagementService_Infra.Migrations
{
    /// <inheritdoc />
    public partial class GOP14UnificaCPFCNPJ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CNPJ",
                table: "USER");

            migrationBuilder.DropColumn(
                name: "CPF",
                table: "USER");

            migrationBuilder.AddColumn<string>(
                name: "CPFCNPJ",
                table: "USER",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CPFCNPJ",
                table: "USER");

            migrationBuilder.AddColumn<string>(
                name: "CNPJ",
                table: "USER",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CPF",
                table: "USER",
                type: "text",
                nullable: true);
        }
    }
}
