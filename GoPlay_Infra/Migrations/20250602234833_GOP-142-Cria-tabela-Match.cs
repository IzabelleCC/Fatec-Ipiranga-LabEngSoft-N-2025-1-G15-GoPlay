using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GoPlay_Infra.Migrations
{
    /// <inheritdoc />
    public partial class GOP142CriatabelaMatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    Player1RegistrationId = table.Column<int>(type: "integer", nullable: false),
                    Player2RegistrationId = table.Column<int>(type: "integer", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Result = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matches_CategoryPlayer_Player1RegistrationId",
                        column: x => x.Player1RegistrationId,
                        principalTable: "CategoryPlayer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matches_CategoryPlayer_Player2RegistrationId",
                        column: x => x.Player2RegistrationId,
                        principalTable: "CategoryPlayer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matches_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_CategoryId",
                table: "Matches",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_Player1RegistrationId",
                table: "Matches",
                column: "Player1RegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_Player2RegistrationId",
                table: "Matches",
                column: "Player2RegistrationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Matches");
        }
    }
}
