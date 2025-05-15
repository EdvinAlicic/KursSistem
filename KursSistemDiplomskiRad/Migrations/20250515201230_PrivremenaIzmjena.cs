using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KursSistemDiplomskiRad.Migrations
{
    /// <inheritdoc />
    public partial class PrivremenaIzmjena : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MedijskiSadrzaj",
                table: "Lekcije",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "MedijskiSadrzaj",
                table: "Lekcije",
                type: "varbinary(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
