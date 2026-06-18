using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrestamosUcayNet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPrestamoIdCobros : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PrestamoID",
                schema: "Prestamos",
                table: "PrestamoDetallePagos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PrestamoID",
                schema: "Prestamos",
                table: "PrestamoCobros",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
                UPDATE d
                SET d.PrestamoID = a.PrestamoID
                FROM [Prestamos].[PrestamoDetallePagos] d
                INNER JOIN [Prestamos].[PrestamosAmortizaciones] a ON a.[AmortizacionID] = d.[AmortizacionId];

                UPDATE c
                SET c.PrestamoID = dp.PrestamoID
                FROM [Prestamos].[PrestamoCobros] c
                INNER JOIN (
                    SELECT [CobroId], MAX([PrestamoID]) AS PrestamoID
                    FROM [Prestamos].[PrestamoDetallePagos]
                    GROUP BY [CobroId]
                ) dp ON dp.[CobroId] = c.[CobroId];

                DECLARE @PrimerPrestamoId INT = (SELECT TOP 1 [PrestamoID] FROM [Prestamos].[Prestamos] ORDER BY [PrestamoID]);

                UPDATE [Prestamos].[PrestamoDetallePagos]
                SET [PrestamoID] = @PrimerPrestamoId
                WHERE [PrestamoID] = 0 AND @PrimerPrestamoId IS NOT NULL;

                UPDATE [Prestamos].[PrestamoCobros]
                SET [PrestamoID] = @PrimerPrestamoId
                WHERE [PrestamoID] = 0 AND @PrimerPrestamoId IS NOT NULL;
            ");

            migrationBuilder.CreateIndex(
                name: "IX_PrestamoDetallePagos_PrestamoID",
                schema: "Prestamos",
                table: "PrestamoDetallePagos",
                column: "PrestamoID");

            migrationBuilder.CreateIndex(
                name: "IX_PrestamoCobros_PrestamoID",
                schema: "Prestamos",
                table: "PrestamoCobros",
                column: "PrestamoID");

            migrationBuilder.AddForeignKey(
                name: "FK_PrestamoCobros_Prestamos",
                schema: "Prestamos",
                table: "PrestamoCobros",
                column: "PrestamoID",
                principalSchema: "Prestamos",
                principalTable: "Prestamos",
                principalColumn: "PrestamoID");

            migrationBuilder.AddForeignKey(
                name: "FK_PrestamoDetallePagos_Prestamos",
                schema: "Prestamos",
                table: "PrestamoDetallePagos",
                column: "PrestamoID",
                principalSchema: "Prestamos",
                principalTable: "Prestamos",
                principalColumn: "PrestamoID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrestamoCobros_Prestamos",
                schema: "Prestamos",
                table: "PrestamoCobros");

            migrationBuilder.DropForeignKey(
                name: "FK_PrestamoDetallePagos_Prestamos",
                schema: "Prestamos",
                table: "PrestamoDetallePagos");

            migrationBuilder.DropIndex(
                name: "IX_PrestamoDetallePagos_PrestamoID",
                schema: "Prestamos",
                table: "PrestamoDetallePagos");

            migrationBuilder.DropIndex(
                name: "IX_PrestamoCobros_PrestamoID",
                schema: "Prestamos",
                table: "PrestamoCobros");

            migrationBuilder.DropColumn(
                name: "PrestamoID",
                schema: "Prestamos",
                table: "PrestamoDetallePagos");

            migrationBuilder.DropColumn(
                name: "PrestamoID",
                schema: "Prestamos",
                table: "PrestamoCobros");
        }
    }
}
