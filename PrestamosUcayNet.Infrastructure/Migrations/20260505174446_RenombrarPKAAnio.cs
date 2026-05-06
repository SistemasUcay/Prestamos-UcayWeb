using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrestamosUcayNet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenombrarPKAAnio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Renombrar la columna PK de ParametrosGeneralesID a Anio
            migrationBuilder.RenameColumn(
                name: "ParametrosGeneralesID",
                schema: "Prestamos",
                table: "ParametrosGenerales",
                newName: "Anio");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Anio",
                schema: "Prestamos",
                table: "ParametrosGenerales",
                newName: "ParametrosGeneralesID");
        }
    }
}
