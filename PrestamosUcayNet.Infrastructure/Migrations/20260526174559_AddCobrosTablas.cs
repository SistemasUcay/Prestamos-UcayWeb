using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrestamosUcayNet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCobrosTablas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrestamoCobros",
                schema: "Prestamos",
                columns: table => new
                {
                    CobroId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaCobro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MontoTotal = table.Column<decimal>(type: "money", nullable: false),
                    MetodoPago = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Referencia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    OrigenNomina = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrestamoCobros", x => x.CobroId);
                });

            migrationBuilder.CreateTable(
                name: "PrestamoDetallePagos",
                schema: "Prestamos",
                columns: table => new
                {
                    DetallePagoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CobroId = table.Column<int>(type: "int", nullable: false),
                    AmortizacionId = table.Column<int>(type: "int", nullable: false),
                    MontoCapital = table.Column<decimal>(type: "money", nullable: false),
                    MontoInteres = table.Column<decimal>(type: "money", nullable: false),
                    PagoExtraCapital = table.Column<decimal>(type: "money", nullable: false),
                    MontoTotal = table.Column<decimal>(type: "money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrestamoDetallePagos", x => x.DetallePagoId);
                    table.ForeignKey(
                        name: "FK_PrestamoDetallePagos_PrestamoCobros",
                        column: x => x.CobroId,
                        principalSchema: "Prestamos",
                        principalTable: "PrestamoCobros",
                        principalColumn: "CobroId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrestamoDetallePagos_PrestamosAmortizaciones",
                        column: x => x.AmortizacionId,
                        principalSchema: "Prestamos",
                        principalTable: "PrestamosAmortizaciones",
                        principalColumn: "AmortizacionID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrestamoDetallePagos_AmortizacionId",
                schema: "Prestamos",
                table: "PrestamoDetallePagos",
                column: "AmortizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_PrestamoDetallePagos_CobroId",
                schema: "Prestamos",
                table: "PrestamoDetallePagos",
                column: "CobroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrestamoDetallePagos",
                schema: "Prestamos");

            migrationBuilder.DropTable(
                name: "PrestamoCobros",
                schema: "Prestamos");
        }
    }
}
