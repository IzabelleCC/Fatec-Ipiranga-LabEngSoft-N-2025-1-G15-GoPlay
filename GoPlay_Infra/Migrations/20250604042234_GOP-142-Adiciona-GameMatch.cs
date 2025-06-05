using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GoPlay_Infra.Migrations
{
    /// <inheritdoc />
    public partial class GOP142AdicionaGameMatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameMatch",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GroupNumber = table.Column<int>(type: "integer", nullable: false),
                    Competitor1Id = table.Column<int>(type: "integer", nullable: false),
                    Competitor2Id = table.Column<int>(type: "integer", nullable: false),
                    MatchStage = table.Column<int>(type: "integer", nullable: false),
                    MatchTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CourtNumber = table.Column<int>(type: "integer", nullable: true),
                    QtdGames1 = table.Column<int>(type: "integer", nullable: false),
                    QtdGames2 = table.Column<int>(type: "integer", nullable: false),
                    Result = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameMatch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameMatch_CategoryPlayer_Competitor1Id",
                        column: x => x.Competitor1Id,
                        principalTable: "CategoryPlayer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameMatch_CategoryPlayer_Competitor2Id",
                        column: x => x.Competitor2Id,
                        principalTable: "CategoryPlayer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameMatch_Competitor1Id",
                table: "GameMatch",
                column: "Competitor1Id");

            migrationBuilder.CreateIndex(
                name: "IX_GameMatch_Competitor2Id",
                table: "GameMatch",
                column: "Competitor2Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameMatch");
        }
    }
}
