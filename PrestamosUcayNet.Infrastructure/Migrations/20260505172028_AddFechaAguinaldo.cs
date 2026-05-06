using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrestamosUcayNet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFechaAguinaldo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAguinaldo",
                schema: "Prestamos",
                table: "ParametrosGenerales",
                type: "smalldatetime",
                nullable: true,
                comment: "Fecha de Entrega de Aguinaldo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaAguinaldo",
                schema: "Prestamos",
                table: "ParametrosGenerales");
        }
    }
}
