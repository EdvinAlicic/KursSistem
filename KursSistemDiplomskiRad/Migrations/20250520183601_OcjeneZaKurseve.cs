using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KursSistemDiplomskiRad.Migrations
{
    /// <inheritdoc />
    public partial class OcjeneZaKurseve : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KursOcjene",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KursId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    Ocjena = table.Column<int>(type: "int", nullable: false),
                    Komentar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KursOcjene", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KursOcjene_Kursevi_KursId",
                        column: x => x.KursId,
                        principalTable: "Kursevi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KursOcjene_Studenti_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Studenti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KursOcjene_KursId",
                table: "KursOcjene",
                column: "KursId");

            migrationBuilder.CreateIndex(
                name: "IX_KursOcjene_StudentId",
                table: "KursOcjene",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KursOcjene");
        }
    }
}
