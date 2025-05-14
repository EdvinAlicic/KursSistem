using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KursSistemDiplomskiRad.Migrations
{
    /// <inheritdoc />
    public partial class NovaTabela : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentKurs",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    KursId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentKurs", x => new { x.StudentId, x.KursId });
                    table.ForeignKey(
                        name: "FK_StudentKurs_Kurs_KursId",
                        column: x => x.KursId,
                        principalTable: "Kurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentKurs_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentKurs_KursId",
                table: "StudentKurs",
                column: "KursId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentKurs");
        }
    }
}
