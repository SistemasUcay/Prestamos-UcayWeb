using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrestamosUcayNet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPrestamosDirectos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Eliminar FK solo si existe
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Prestamos_Solicitud')
                    ALTER TABLE [Prestamos].[Prestamos] DROP CONSTRAINT [FK_Prestamos_Solicitud];");

            // Eliminar tablas solo si existen
            migrationBuilder.Sql(@"
                IF OBJECT_ID('[Prestamos].[Pagos]', 'U') IS NOT NULL
                    DROP TABLE [Prestamos].[Pagos];");

            migrationBuilder.Sql(@"
                IF OBJECT_ID('[Prestamos].[PausasPrestamo]', 'U') IS NOT NULL
                    DROP TABLE [Prestamos].[PausasPrestamo];");

            migrationBuilder.Sql(@"
                IF OBJECT_ID('[Prestamos].[SolicitudesPrestamo]', 'U') IS NOT NULL
                    DROP TABLE [Prestamos].[SolicitudesPrestamo];");

            // Eliminar índice solo si existe
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Prestamos_SolicitudId' AND object_id = OBJECT_ID('[Prestamos].[Prestamos]'))
                    DROP INDEX [IX_Prestamos_SolicitudId] ON [Prestamos].[Prestamos];");

            // Eliminar columna solo si existe
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE name = 'SolicitudId' AND object_id = OBJECT_ID('[Prestamos].[Prestamos]'))
                    ALTER TABLE [Prestamos].[Prestamos] DROP COLUMN [SolicitudId];");

            // Agregar columnas solo si no existen
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE name = 'FechaUtilidades' AND object_id = OBJECT_ID('[Prestamos].[ParametrosGenerales]'))
                BEGIN
                    ALTER TABLE [Prestamos].[ParametrosGenerales] ADD [FechaUtilidades] smalldatetime NULL;
                    EXEC sp_addextendedproperty 'MS_Description', N'Fecha de Entrega de Utilidades',
                        'SCHEMA', N'Prestamos', 'TABLE', N'ParametrosGenerales', 'COLUMN', N'FechaUtilidades';
                END");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE name = 'TasaFijaDirecto' AND object_id = OBJECT_ID('[Prestamos].[ParametrosGenerales]'))
                BEGIN
                    ALTER TABLE [Prestamos].[ParametrosGenerales] ADD [TasaFijaDirecto] decimal(8,2) NULL;
                    EXEC sp_addextendedproperty 'MS_Description', N'Tasa Fija de Préstamos Directos',
                        'SCHEMA', N'Prestamos', 'TABLE', N'ParametrosGenerales', 'COLUMN', N'TasaFijaDirecto';
                END");

            // Crear tabla PrestamosDirectos solo si no existe
            migrationBuilder.Sql(@"
                IF OBJECT_ID('[Prestamos].[PrestamosDirectos]', 'U') IS NULL
                BEGIN
                    CREATE TABLE [Prestamos].[PrestamosDirectos] (
                        [NumPrestamoDirecto] int IDENTITY(1,1) NOT NULL,
                        [PrestamoId]         int NOT NULL,
                        [Descripcion]        varchar(250) NOT NULL,
                        [MontoSolicitado]    money NOT NULL,
                        [TasaMensual]        decimal(8,4) NOT NULL,
                        [FechaPago]          smalldatetime NOT NULL,
                        [DiazPlazo]          int NOT NULL,
                        [MontoIntereses]     money NOT NULL,
                        [TotalCapitalInteres] money NOT NULL,
                        CONSTRAINT [PK_PrestamosDirectos] PRIMARY KEY ([NumPrestamoDirecto]),
                        CONSTRAINT [FK_PrestamosDirectos_Prestamos] FOREIGN KEY ([PrestamoId])
                            REFERENCES [Prestamos].[Prestamos] ([PrestamoID]) ON DELETE CASCADE
                    );
                    CREATE INDEX [IX_PrestamosDirectos_PrestamoId] ON [Prestamos].[PrestamosDirectos] ([PrestamoId]);
                END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrestamosDirectos",
                schema: "Prestamos");

            migrationBuilder.DropColumn(
                name: "FechaUtilidades",
                schema: "Prestamos",
                table: "ParametrosGenerales");

            migrationBuilder.DropColumn(
                name: "TasaFijaDirecto",
                schema: "Prestamos",
                table: "ParametrosGenerales");

            migrationBuilder.AddColumn<int>(
                name: "SolicitudId",
                schema: "Prestamos",
                table: "Prestamos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Pagos",
                schema: "Prestamos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrestamoId = table.Column<int>(type: "int", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MetodoPago = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MontoPagado = table.Column<decimal>(type: "money", nullable: false),
                    Notas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Referencia = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    TipoPago = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pagos_Prestamos",
                        column: x => x.PrestamoId,
                        principalSchema: "Prestamos",
                        principalTable: "Prestamos",
                        principalColumn: "PrestamoID");
                });

            migrationBuilder.CreateTable(
                name: "PausasPrestamo",
                schema: "Prestamos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrestamoId = table.Column<int>(type: "int", nullable: false),
                    Activa = table.Column<bool>(type: "bit", nullable: false),
                    AutorizadoPorUserId = table.Column<int>(type: "int", nullable: true),
                    DuracionMeses = table.Column<int>(type: "int", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Motivo = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PausasPrestamo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pausas_Prestamos",
                        column: x => x.PrestamoId,
                        principalSchema: "Prestamos",
                        principalTable: "Prestamos",
                        principalColumn: "PrestamoID");
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesPrestamo",
                schema: "Prestamos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CondicionCreditoId = table.Column<int>(type: "int", nullable: false),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    EmpresaEmpleadoraId = table.Column<int>(type: "int", nullable: false),
                    FondeadoraId = table.Column<int>(type: "int", nullable: false),
                    AprobadoPorUserId = table.Column<int>(type: "int", nullable: true),
                    EstadoSolicitud = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MontoSolicitado = table.Column<decimal>(type: "money", nullable: false),
                    Motivo = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    MotivoCancelacion = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    MotivoRechazo = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    PlazoMeses = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesPrestamo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Solicitudes_Condiciones",
                        column: x => x.CondicionCreditoId,
                        principalSchema: "Prestamos",
                        principalTable: "CondicionesCredito",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Solicitudes_Empleados",
                        column: x => x.EmpleadoId,
                        principalSchema: "Prestamos",
                        principalTable: "Empleados",
                        principalColumn: "EmpleadoID");
                    table.ForeignKey(
                        name: "FK_Solicitudes_EmpresaFondeadora",
                        column: x => x.FondeadoraId,
                        principalSchema: "Prestamos",
                        principalTable: "DatosEmpresa",
                        principalColumn: "NumEmpresa");
                    table.ForeignKey(
                        name: "FK_Solicitudes_EmpresaLaboral",
                        column: x => x.EmpresaEmpleadoraId,
                        principalSchema: "Prestamos",
                        principalTable: "DatosEmpresa",
                        principalColumn: "NumEmpresa");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_SolicitudId",
                schema: "Prestamos",
                table: "Prestamos",
                column: "SolicitudId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_PrestamoId",
                schema: "Prestamos",
                table: "Pagos",
                column: "PrestamoId");

            migrationBuilder.CreateIndex(
                name: "IX_PausasPrestamo_PrestamoId",
                schema: "Prestamos",
                table: "PausasPrestamo",
                column: "PrestamoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesPrestamo_CondicionCreditoId",
                schema: "Prestamos",
                table: "SolicitudesPrestamo",
                column: "CondicionCreditoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesPrestamo_EmpleadoId",
                schema: "Prestamos",
                table: "SolicitudesPrestamo",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesPrestamo_EmpresaEmpleadoraId",
                schema: "Prestamos",
                table: "SolicitudesPrestamo",
                column: "EmpresaEmpleadoraId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesPrestamo_FondeadoraId",
                schema: "Prestamos",
                table: "SolicitudesPrestamo",
                column: "FondeadoraId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prestamos_Solicitud",
                schema: "Prestamos",
                table: "Prestamos",
                column: "SolicitudId",
                principalSchema: "Prestamos",
                principalTable: "SolicitudesPrestamo",
                principalColumn: "Id");
        }
    }
}
