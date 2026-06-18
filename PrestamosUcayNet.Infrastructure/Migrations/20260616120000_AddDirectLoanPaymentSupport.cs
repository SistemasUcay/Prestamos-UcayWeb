using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrestamosUcayNet.Infrastructure.Migrations
{
    public partial class AddDirectLoanPaymentSupport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrestamoDetallePagos_PrestamosAmortizaciones",
                schema: "Prestamos",
                table: "PrestamoDetallePagos");

            migrationBuilder.AlterColumn<int>(
                name: "AmortizacionID",
                schema: "Prestamos",
                table: "PrestamoDetallePagos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "PrestamoDirectoID",
                schema: "Prestamos",
                table: "PrestamoDetallePagos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "StatusPago",
                schema: "Prestamos",
                table: "PrestamosDirectos",
                type: "bit",
                nullable: false,
                defaultValue: false)
                .Annotation("Relational:Comment", "Indica si el préstamo directo ya fue pagado");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaPagoReal",
                schema: "Prestamos",
                table: "PrestamosDirectos",
                type: "smalldatetime",
                nullable: true)
                .Annotation("Relational:Comment", "Fecha en que se registró el cobro del préstamo directo");

            migrationBuilder.CreateIndex(
                name: "IX_PrestamoDetallePagos_PrestamoDirectoID",
                schema: "Prestamos",
                table: "PrestamoDetallePagos",
                column: "PrestamoDirectoID");

            migrationBuilder.AddForeignKey(
                name: "FK_PrestamoDetallePagos_PrestamosAmortizaciones",
                schema: "Prestamos",
                table: "PrestamoDetallePagos",
                column: "AmortizacionID",
                principalSchema: "Prestamos",
                principalTable: "PrestamosAmortizaciones",
                principalColumn: "AmortizacionID",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_PrestamoDetallePagos_PrestamosDirectos",
                schema: "Prestamos",
                table: "PrestamoDetallePagos",
                column: "PrestamoDirectoID",
                principalSchema: "Prestamos",
                principalTable: "PrestamosDirectos",
                principalColumn: "NumPrestamoDirecto",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrestamoDetallePagos_PrestamosAmortizaciones",
                schema: "Prestamos",
                table: "PrestamoDetallePagos");

            migrationBuilder.DropForeignKey(
                name: "FK_PrestamoDetallePagos_PrestamosDirectos",
                schema: "Prestamos",
                table: "PrestamoDetallePagos");

            migrationBuilder.DropIndex(
                name: "IX_PrestamoDetallePagos_PrestamoDirectoID",
                schema: "Prestamos",
                table: "PrestamoDetallePagos");

            migrationBuilder.DropColumn(
                name: "PrestamoDirectoID",
                schema: "Prestamos",
                table: "PrestamoDetallePagos");

            migrationBuilder.DropColumn(
                name: "StatusPago",
                schema: "Prestamos",
                table: "PrestamosDirectos");

            migrationBuilder.DropColumn(
                name: "FechaPagoReal",
                schema: "Prestamos",
                table: "PrestamosDirectos");

            migrationBuilder.AlterColumn<int>(
                name: "AmortizacionID",
                schema: "Prestamos",
                table: "PrestamoDetallePagos",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PrestamoDetallePagos_PrestamosAmortizaciones",
                schema: "Prestamos",
                table: "PrestamoDetallePagos",
                column: "AmortizacionID",
                principalSchema: "Prestamos",
                principalTable: "PrestamosAmortizaciones",
                principalColumn: "AmortizacionID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
