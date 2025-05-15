using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KursSistemDiplomskiRad.Migrations
{
    /// <inheritdoc />
    public partial class NoveIzmjene : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lekcije_Kurs_KursId",
                table: "Lekcije");

            migrationBuilder.DropForeignKey(
                name: "FK_Student_Kurs_KursId",
                table: "Student");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentKurs_Kurs_KursId",
                table: "StudentKurs");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentKurs_Student_StudentId",
                table: "StudentKurs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Student",
                table: "Student");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Kurs",
                table: "Kurs");

            migrationBuilder.RenameTable(
                name: "Student",
                newName: "Studenti");

            migrationBuilder.RenameTable(
                name: "Kurs",
                newName: "Kursevi");

            migrationBuilder.RenameIndex(
                name: "IX_Student_KursId",
                table: "Studenti",
                newName: "IX_Studenti_KursId");

            migrationBuilder.AddColumn<DateTime>(
                name: "DatumPrijave",
                table: "StudentKurs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "StatusPrijave",
                table: "StudentKurs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Studenti",
                table: "Studenti",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Kursevi",
                table: "Kursevi",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lekcije_Kursevi_KursId",
                table: "Lekcije",
                column: "KursId",
                principalTable: "Kursevi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Studenti_Kursevi_KursId",
                table: "Studenti",
                column: "KursId",
                principalTable: "Kursevi",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentKurs_Kursevi_KursId",
                table: "StudentKurs",
                column: "KursId",
                principalTable: "Kursevi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentKurs_Studenti_StudentId",
                table: "StudentKurs",
                column: "StudentId",
                principalTable: "Studenti",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lekcije_Kursevi_KursId",
                table: "Lekcije");

            migrationBuilder.DropForeignKey(
                name: "FK_Studenti_Kursevi_KursId",
                table: "Studenti");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentKurs_Kursevi_KursId",
                table: "StudentKurs");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentKurs_Studenti_StudentId",
                table: "StudentKurs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Studenti",
                table: "Studenti");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Kursevi",
                table: "Kursevi");

            migrationBuilder.DropColumn(
                name: "DatumPrijave",
                table: "StudentKurs");

            migrationBuilder.DropColumn(
                name: "StatusPrijave",
                table: "StudentKurs");

            migrationBuilder.RenameTable(
                name: "Studenti",
                newName: "Student");

            migrationBuilder.RenameTable(
                name: "Kursevi",
                newName: "Kurs");

            migrationBuilder.RenameIndex(
                name: "IX_Studenti_KursId",
                table: "Student",
                newName: "IX_Student_KursId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Student",
                table: "Student",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Kurs",
                table: "Kurs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lekcije_Kurs_KursId",
                table: "Lekcije",
                column: "KursId",
                principalTable: "Kurs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Student_Kurs_KursId",
                table: "Student",
                column: "KursId",
                principalTable: "Kurs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentKurs_Kurs_KursId",
                table: "StudentKurs",
                column: "KursId",
                principalTable: "Kurs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentKurs_Student_StudentId",
                table: "StudentKurs",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
