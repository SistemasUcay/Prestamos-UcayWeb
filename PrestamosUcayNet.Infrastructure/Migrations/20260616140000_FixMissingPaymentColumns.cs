using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrestamosUcayNet.Infrastructure.Migrations
{
    public partial class FixMissingPaymentColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "StatusPago",
                schema: "Prestamos",
                table: "PrestamosDirectos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaPagoReal",
                schema: "Prestamos",
                table: "PrestamosDirectos",
                type: "datetime2",
                nullable: true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaPagoReal",
                schema: "Prestamos",
                table: "PrestamosDirectos");

            migrationBuilder.DropColumn(
                name: "StatusPago",
                schema: "Prestamos",
                table: "PrestamosDirectos");

        }
    }
}
