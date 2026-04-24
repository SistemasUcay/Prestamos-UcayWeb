using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrestamosUcayNet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeSolicitudNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Clasificacion",
                schema: "Prestamos",
                table: "TipoPrestamo",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Clasificación del Tipo de Préstamo [1]:Corto Plazo [2]:Largo Plazo [3]:Préstamo Hipotecario [4]:Préstamo Directo",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Clasificación del Préstamo [1]:Corto Plazo [2]:Largo  Plazo [3]:Préstamo Hipotecario");

            migrationBuilder.AlterColumn<int>(
                name: "SolicitudId",
                schema: "Prestamos",
                table: "Prestamos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Clasificacion",
                schema: "Prestamos",
                table: "TipoPrestamo",
                type: "int",
                nullable: true,
                comment: "Clasificación del Préstamo [1]:Corto Plazo [2]:Largo  Plazo [3]:Préstamo Hipotecario",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Clasificación del Tipo de Préstamo [1]:Corto Plazo [2]:Largo Plazo [3]:Préstamo Hipotecario [4]:Préstamo Directo");

            migrationBuilder.AlterColumn<int>(
                name: "SolicitudId",
                schema: "Prestamos",
                table: "Prestamos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
