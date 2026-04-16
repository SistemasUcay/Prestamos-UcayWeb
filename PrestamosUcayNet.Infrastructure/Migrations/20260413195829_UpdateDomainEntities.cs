using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrestamosUcayNet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDomainEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Prestamos");

            migrationBuilder.CreateTable(
                name: "DatosEmpresa",
                schema: "Prestamos",
                columns: table => new
                {
                    NumEmpresa = table.Column<int>(type: "int", nullable: false, comment: "Identidicador de la Empresa")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RazonSocial = table.Column<string>(type: "varchar(70)", unicode: false, maxLength: 70, nullable: false, comment: "Razón Social de la Empresa"),
                    NombreCorto = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true, comment: "Nombre Corto de la Empresa"),
                    Direccion = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true, comment: "Domicilio Completo de la Empresa"),
                    CodPostal = table.Column<string>(type: "nchar(5)", fixedLength: true, maxLength: 5, nullable: true, comment: "Código Postal del domicilio de la empresa"),
                    RFC = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false, comment: "RFC / NIT"),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RepresentanteLegal = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsCajaDeAhorro = table.Column<bool>(type: "bit", nullable: false),
                    Activa = table.Column<bool>(type: "bit", nullable: false),
                    SaldoActual = table.Column<decimal>(type: "money", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatosEmpresa", x => x.NumEmpresa)
                        .Annotation("SqlServer:FillFactor", 90);
                });

            migrationBuilder.CreateTable(
                name: "Departamentos",
                schema: "Prestamos",
                columns: table => new
                {
                    DepartamentoID = table.Column<int>(type: "int", nullable: false, comment: "Identificador del Departamento")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true, comment: "Descripción del Departamento")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departamentos", x => x.DepartamentoID);
                });

            migrationBuilder.CreateTable(
                name: "ParametrosGenerales",
                schema: "Prestamos",
                columns: table => new
                {
                    ParametrosGeneralesID = table.Column<int>(type: "int", nullable: false, comment: "Identificador de Parametros"),
                    InicioPeriodoSemanal = table.Column<DateTime>(type: "smalldatetime", nullable: true, comment: "Fecha Inicial del primer periodo semanal del Año para el sistema de Nómina"),
                    InicioPeriodoCatorcenal = table.Column<DateTime>(type: "smalldatetime", nullable: true, comment: "Fecha Inicial del primer periodo catorcenal del Año para el sistema de Nómina"),
                    TasaFijaEmpresa = table.Column<decimal>(type: "decimal(8,2)", nullable: true, comment: "Tasa Fija para Préstamos de Empresa"),
                    TasaFijaCaja = table.Column<decimal>(type: "decimal(8,2)", nullable: true, comment: "Tasa Fija de Préstamos de la Caja de Ahorro"),
                    CierreCajaAnual = table.Column<DateTime>(type: "smalldatetime", nullable: true, comment: "Fecha de Cierre para la Caja de Ahorro")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParametrosGenerales", x => x.ParametrosGeneralesID);
                });

            migrationBuilder.CreateTable(
                name: "Puestos",
                schema: "Prestamos",
                columns: table => new
                {
                    PuestoID = table.Column<int>(type: "int", nullable: false, comment: "Registo ID del Puesto")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true, comment: "Descripción del Puesto")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Puestos", x => x.PuestoID);
                },
                comment: "Descripción de Puestos");

            migrationBuilder.CreateTable(
                name: "TipoPrestamo",
                schema: "Prestamos",
                columns: table => new
                {
                    TipoPrestamoID = table.Column<int>(type: "int", nullable: false, comment: "Identificador del Tipo de Préstamo")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, comment: "Descripción del Tipo Préstamo "),
                    Clasificacion = table.Column<int>(type: "int", nullable: true, comment: "Clasificación del Préstamo [1]:Corto Plazo [2]:Largo  Plazo [3]:Préstamo Hipotecario")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoPrestamo", x => x.TipoPrestamoID);
                },
                comment: "Desfine los Tipo de Préstamos");

            migrationBuilder.CreateTable(
                name: "TiposPago",
                schema: "Prestamos",
                columns: table => new
                {
                    TipoPagoID = table.Column<int>(type: "int", nullable: false, comment: "Identificador del Tipo de Pago")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, comment: "Descripción Tipo de Pago"),
                    IntervaloDiario = table.Column<int>(type: "int", nullable: false, comment: "Número de Días para la frecuencia de pago"),
                    IntervaloMensual = table.Column<int>(type: "int", nullable: false, comment: "Número de Meses para la frecuencia del pago"),
                    IntervaloAnual = table.Column<int>(type: "int", nullable: false, comment: "Intervalo Anual "),
                    FrecuenciaAnual = table.Column<int>(type: "int", nullable: false, comment: "Níumero de Pagos por año")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposPago", x => x.TipoPagoID);
                });

            migrationBuilder.CreateTable(
                name: "CondicionesCredito",
                schema: "Prestamos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FondeadoraId = table.Column<int>(type: "int", nullable: false),
                    NombreProducto = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    TasaInteresMensual = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    PlazoMaximoMeses = table.Column<int>(type: "int", nullable: false),
                    MontoMaximo = table.Column<decimal>(type: "money", nullable: false),
                    MontoMinimo = table.Column<decimal>(type: "money", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CondicionesCredito", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Condiciones_DatosEmpresa",
                        column: x => x.FondeadoraId,
                        principalSchema: "Prestamos",
                        principalTable: "DatosEmpresa",
                        principalColumn: "NumEmpresa");
                });

            migrationBuilder.CreateTable(
                name: "Empleados",
                schema: "Prestamos",
                columns: table => new
                {
                    EmpleadoID = table.Column<int>(type: "int", nullable: false, comment: "Identidicador el Empleado")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombres = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, comment: "Nombre(s) del Emplado"),
                    ApellidoPaterno = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, comment: "Apellido Paterno"),
                    ApellidoMaterno = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true, comment: "Apellido Materno"),
                    RFC = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false, comment: "RFC"),
                    CURP = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true, comment: "CURP"),
                    Nacimiento = table.Column<DateTime>(type: "datetime", nullable: false, comment: "Fecha de Nacimiento"),
                    NumeroNomina = table.Column<int>(type: "int", nullable: false, comment: "Número del sistema de nómina del empleado"),
                    Domicilio = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false, comment: "Nombre de la calle y número"),
                    CodigoPostal = table.Column<int>(type: "int", nullable: false, comment: "Código Postal del Domicilio"),
                    Celular = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true, comment: "Número del Celular"),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true, comment: "Correo Electrónico"),
                    CuentaBancaria = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true, comment: "Cuenta Bancaria de Depósitos"),
                    CuentaClabe = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true, comment: "Número CLABE de la Cuenta de Depósito"),
                    FechaIngreso = table.Column<DateTime>(type: "datetime", nullable: true, comment: "Fecha de Ingreso a la Empresa"),
                    SueldoNominal = table.Column<decimal>(type: "money", nullable: true, comment: "Sueldo Nominal"),
                    SueldoLaboral = table.Column<decimal>(type: "money", nullable: true, comment: "Sueldo Laboral"),
                    EmpresaID = table.Column<int>(type: "int", nullable: false, comment: "ID de la Empresa donde labora"),
                    DepartamentoID = table.Column<int>(type: "int", nullable: false, comment: "ID del Departamento "),
                    PuestoID = table.Column<int>(type: "int", nullable: false, comment: "Puesto donde labora el empleado"),
                    TipoPagoID = table.Column<int>(type: "int", nullable: false, comment: "Tipo de Pago de la Nómina"),
                    ImporteCajaAhorro = table.Column<decimal>(type: "money", nullable: false, comment: "Importe para la Caja de Ahorro"),
                    EmpleadoUcay = table.Column<bool>(type: "bit", nullable: false, comment: "Inidica si el empleado es directo de UCAY")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleados", x => x.EmpleadoID);
                    table.ForeignKey(
                        name: "FK_Empleados_DatosEmpresa",
                        column: x => x.EmpresaID,
                        principalSchema: "Prestamos",
                        principalTable: "DatosEmpresa",
                        principalColumn: "NumEmpresa");
                    table.ForeignKey(
                        name: "FK_Empleados_Departamentos",
                        column: x => x.DepartamentoID,
                        principalSchema: "Prestamos",
                        principalTable: "Departamentos",
                        principalColumn: "DepartamentoID");
                    table.ForeignKey(
                        name: "FK_Empleados_Puestos",
                        column: x => x.PuestoID,
                        principalSchema: "Prestamos",
                        principalTable: "Puestos",
                        principalColumn: "PuestoID");
                    table.ForeignKey(
                        name: "FK_Empleados_TiposPago",
                        column: x => x.TipoPagoID,
                        principalSchema: "Prestamos",
                        principalTable: "TiposPago",
                        principalColumn: "TipoPagoID");
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesPrestamo",
                schema: "Prestamos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    EmpresaEmpleadoraId = table.Column<int>(type: "int", nullable: false),
                    FondeadoraId = table.Column<int>(type: "int", nullable: false),
                    CondicionCreditoId = table.Column<int>(type: "int", nullable: false),
                    MontoSolicitado = table.Column<decimal>(type: "money", nullable: false),
                    PlazoMeses = table.Column<int>(type: "int", nullable: false),
                    Motivo = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    EstadoSolicitud = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MotivoRechazo = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    MotivoCancelacion = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    AprobadoPorUserId = table.Column<int>(type: "int", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "Prestamos",
                schema: "Prestamos",
                columns: table => new
                {
                    PrestamoID = table.Column<int>(type: "int", nullable: false, comment: "Identificador del Préstamo")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaSolicitud = table.Column<DateTime>(type: "smalldatetime", nullable: false, comment: "Fecha de Solicitud del Préstamo"),
                    SolicitudId = table.Column<int>(type: "int", nullable: false),
                    FuenteFondeoID = table.Column<int>(type: "int", nullable: false, comment: "Fuente de Fondeo Proveedor del capital para el financiamiento"),
                    EmpleadoID = table.Column<int>(type: "int", nullable: false, comment: "ID del Empleador que solicita el Préstamo"),
                    SueldoLaboral = table.Column<decimal>(type: "money", nullable: false, comment: "Sueldo Laboral del Empleado al momento de Solicitar el Préstamo"),
                    TipoPrestamoID = table.Column<int>(type: "int", nullable: false, comment: "ID del Tipo de Préstamo"),
                    TipoPlazo = table.Column<int>(type: "int", nullable: true, comment: "Tipos de Plazo: [1]: Corto Plazo [2]: Largo Plazo [3]: Hipotecario"),
                    TasaInteresAnual = table.Column<decimal>(type: "decimal(8,2)", nullable: false, comment: "Tasa de Interes Anual del Préstamo"),
                    MontoAprobado = table.Column<decimal>(type: "money", nullable: false, comment: "Importe Aprobado/Entregado"),
                    PlazoMeses = table.Column<int>(type: "int", nullable: false, comment: "Plazo del Préstamo en Meses"),
                    TasaInteresMensual = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DestinoCredito = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false, comment: "Definición del destino del Préstamo"),
                    TipoPagoID = table.Column<int>(type: "int", nullable: false, comment: "Tipo de Pago o Amortización del Préstamo (Describe la frecuencia del Pago)"),
                    NumeroCuotas = table.Column<int>(type: "int", nullable: false, comment: "Número de Pagos o Cuotas Totales del Préstamo"),
                    ImporteCuotas = table.Column<decimal>(type: "money", nullable: false, comment: "Importe o Monto del Pago en cada Frecuencia (Pago Programado)"),
                    FechaAprobado = table.Column<DateTime>(type: "smalldatetime", nullable: false, comment: "Fecha de Aporabación del Prestamo"),
                    FechaDeposito = table.Column<DateTime>(type: "smalldatetime", nullable: true, comment: "Fecha de Depósito del Préstamo"),
                    InicioDescuento = table.Column<DateTime>(type: "smalldatetime", nullable: false, comment: "Fecha de inicio del descuento"),
                    FinalDescuento = table.Column<DateTime>(type: "smalldatetime", nullable: false, comment: "Fecha Final del Préstamo"),
                    AvalCredito = table.Column<bool>(type: "bit", nullable: false, comment: "Indica si el Préstamo cuenta con Aval"),
                    EmpleadoIDAval = table.Column<int>(type: "int", nullable: false, comment: "ID del Empleado que será el Aval"),
                    Estado = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false, comment: "Estado del Préstamo"),
                    MetodoEntrega = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenciaBancaria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Folio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalPagar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SaldoPendiente = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prestamos", x => x.PrestamoID);
                    table.ForeignKey(
                        name: "FK_Prestamos_DatosEmpresa",
                        column: x => x.FuenteFondeoID,
                        principalSchema: "Prestamos",
                        principalTable: "DatosEmpresa",
                        principalColumn: "NumEmpresa");
                    table.ForeignKey(
                        name: "FK_Prestamos_Empleados",
                        column: x => x.EmpleadoID,
                        principalSchema: "Prestamos",
                        principalTable: "Empleados",
                        principalColumn: "EmpleadoID");
                    table.ForeignKey(
                        name: "FK_Prestamos_Solicitud",
                        column: x => x.SolicitudId,
                        principalSchema: "Prestamos",
                        principalTable: "SolicitudesPrestamo",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Prestamos_TipoPrestamo",
                        column: x => x.TipoPrestamoID,
                        principalSchema: "Prestamos",
                        principalTable: "TipoPrestamo",
                        principalColumn: "TipoPrestamoID");
                    table.ForeignKey(
                        name: "FK_Prestamos_TiposPago",
                        column: x => x.TipoPagoID,
                        principalSchema: "Prestamos",
                        principalTable: "TiposPago",
                        principalColumn: "TipoPagoID");
                });

            migrationBuilder.CreateTable(
                name: "Pagos",
                schema: "Prestamos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrestamoId = table.Column<int>(type: "int", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MontoPagado = table.Column<decimal>(type: "money", nullable: false),
                    TipoPago = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MetodoPago = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Referencia = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Notas = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DuracionMeses = table.Column<int>(type: "int", nullable: false),
                    Motivo = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    AutorizadoPorUserId = table.Column<int>(type: "int", nullable: true),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activa = table.Column<bool>(type: "bit", nullable: false)
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
                name: "PrestamosAmortizaciones",
                schema: "Prestamos",
                columns: table => new
                {
                    AmortizacionID = table.Column<int>(type: "int", nullable: false, comment: "Identificador de la Amortización")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrestamoID = table.Column<int>(type: "int", nullable: false, comment: "ID del Préstamo "),
                    NumeroPago = table.Column<int>(type: "int", nullable: false, comment: "Número del Pago o Cuota Programado"),
                    FechaPago = table.Column<DateTime>(type: "smalldatetime", nullable: false, comment: "Fecha de Pago Programado"),
                    InicioPeriodoPago = table.Column<DateTime>(type: "smalldatetime", nullable: false, comment: "Feha de Inicio del Periodo para Nómina"),
                    DescripcionPago = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, comment: "Descripción del Pago"),
                    TasaInteres = table.Column<decimal>(type: "money", nullable: true, comment: "Tasa de interés del cálculo"),
                    DiasCalculo = table.Column<int>(type: "int", nullable: true, comment: "Días del Cálculo para el Interés base del Cálculo SaldoInicial"),
                    SaldoInicial = table.Column<decimal>(type: "money", nullable: false, comment: "Importe del Saldo Inicial del Préstamo"),
                    ImportePago = table.Column<decimal>(type: "money", nullable: false, comment: "Importe del Pago o Cuota Programada"),
                    ImportePagoCapital = table.Column<decimal>(type: "money", nullable: false, comment: "Importe del Pago a Capital Programado "),
                    ImportePagoInteres = table.Column<decimal>(type: "money", nullable: false, comment: "Importe del Pago a Interés Programado "),
                    PagoExtraCapital = table.Column<decimal>(type: "money", nullable: false),
                    SaldoFinal = table.Column<decimal>(type: "money", nullable: false, comment: "Saldo de Capital"),
                    StatusPago = table.Column<bool>(type: "bit", nullable: false, comment: "Indica si el Pago ha sido aplicado")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrestamosAmortizaciones", x => x.AmortizacionID);
                    table.ForeignKey(
                        name: "FK_PrestamosAmortizaciones_Prestamos",
                        column: x => x.PrestamoID,
                        principalSchema: "Prestamos",
                        principalTable: "Prestamos",
                        principalColumn: "PrestamoID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CondicionesCredito_FondeadoraId",
                schema: "Prestamos",
                table: "CondicionesCredito",
                column: "FondeadoraId");

            migrationBuilder.CreateIndex(
                name: "IX_Departamentos",
                schema: "Prestamos",
                table: "Departamentos",
                column: "Descripcion",
                unique: true,
                filter: "[Descripcion] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_1",
                schema: "Prestamos",
                table: "Empleados",
                column: "RFC",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_DepartamentoID",
                schema: "Prestamos",
                table: "Empleados",
                column: "DepartamentoID");

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_EmpresaID",
                schema: "Prestamos",
                table: "Empleados",
                column: "EmpresaID");

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_PuestoID",
                schema: "Prestamos",
                table: "Empleados",
                column: "PuestoID");

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_TipoPagoID",
                schema: "Prestamos",
                table: "Empleados",
                column: "TipoPagoID");

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
                name: "IX_Prestamos_EmpleadoID",
                schema: "Prestamos",
                table: "Prestamos",
                column: "EmpleadoID");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_FuenteFondeoID",
                schema: "Prestamos",
                table: "Prestamos",
                column: "FuenteFondeoID");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_SolicitudId",
                schema: "Prestamos",
                table: "Prestamos",
                column: "SolicitudId");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_TipoPagoID",
                schema: "Prestamos",
                table: "Prestamos",
                column: "TipoPagoID");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_TipoPrestamoID",
                schema: "Prestamos",
                table: "Prestamos",
                column: "TipoPrestamoID");

            migrationBuilder.CreateIndex(
                name: "IX_PrestamosAmortizaciones_PrestamoID",
                schema: "Prestamos",
                table: "PrestamosAmortizaciones",
                column: "PrestamoID");

            migrationBuilder.CreateIndex(
                name: "IX_Puestos",
                schema: "Prestamos",
                table: "Puestos",
                column: "Descripcion",
                unique: true,
                filter: "[Descripcion] IS NOT NULL");

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

            migrationBuilder.CreateIndex(
                name: "IX_TipoPrestamo",
                schema: "Prestamos",
                table: "TipoPrestamo",
                column: "Descripcion");

            migrationBuilder.CreateIndex(
                name: "IX_TiposPago",
                schema: "Prestamos",
                table: "TiposPago",
                column: "Descripcion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pagos",
                schema: "Prestamos");

            migrationBuilder.DropTable(
                name: "ParametrosGenerales",
                schema: "Prestamos");

            migrationBuilder.DropTable(
                name: "PausasPrestamo",
                schema: "Prestamos");

            migrationBuilder.DropTable(
                name: "PrestamosAmortizaciones",
                schema: "Prestamos");

            migrationBuilder.DropTable(
                name: "Prestamos",
                schema: "Prestamos");

            migrationBuilder.DropTable(
                name: "SolicitudesPrestamo",
                schema: "Prestamos");

            migrationBuilder.DropTable(
                name: "TipoPrestamo",
                schema: "Prestamos");

            migrationBuilder.DropTable(
                name: "CondicionesCredito",
                schema: "Prestamos");

            migrationBuilder.DropTable(
                name: "Empleados",
                schema: "Prestamos");

            migrationBuilder.DropTable(
                name: "DatosEmpresa",
                schema: "Prestamos");

            migrationBuilder.DropTable(
                name: "Departamentos",
                schema: "Prestamos");

            migrationBuilder.DropTable(
                name: "Puestos",
                schema: "Prestamos");

            migrationBuilder.DropTable(
                name: "TiposPago",
                schema: "Prestamos");
        }
    }
}
