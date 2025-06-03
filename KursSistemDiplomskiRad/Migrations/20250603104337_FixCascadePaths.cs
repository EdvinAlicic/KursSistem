using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KursSistemDiplomskiRad.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadePaths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentLekcijaProgress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    KursId = table.Column<int>(type: "int", nullable: false),
                    LekcijaId = table.Column<int>(type: "int", nullable: false),
                    JeZavrsena = table.Column<bool>(type: "bit", nullable: false),
                    DatumZavrsetka = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentLekcijaProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentLekcijaProgress_Kursevi_KursId",
                        column: x => x.KursId,
                        principalTable: "Kursevi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentLekcijaProgress_Lekcije_LekcijaId",
                        column: x => x.LekcijaId,
                        principalTable: "Lekcije",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentLekcijaProgress_Studenti_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Studenti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentLekcijaProgress_KursId",
                table: "StudentLekcijaProgress",
                column: "KursId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLekcijaProgress_LekcijaId",
                table: "StudentLekcijaProgress",
                column: "LekcijaId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLekcijaProgress_StudentId",
                table: "StudentLekcijaProgress",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentLekcijaProgress");
        }
    }
}
