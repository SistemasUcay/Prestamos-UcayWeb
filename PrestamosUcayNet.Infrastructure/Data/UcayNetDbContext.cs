using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PrestamosUcayNet.Infrastructure;
using PrestamosUcayNet.Infrastructure.Identity;
using PrestamosUcayNet.Domain.Entities;

namespace PrestamosUcayNet.Infrastructure.Data;

public partial class UcayNetDbContext : IdentityDbContext<UsuarioSistema>
{
    public UcayNetDbContext()
    {
    }

    public UcayNetDbContext(DbContextOptions<UcayNetDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DatosEmpresa> DatosEmpresas { get; set; }

    public virtual DbSet<Departamento> Departamentos { get; set; }

    public virtual DbSet<Empleado> Empleados { get; set; }

    public virtual DbSet<ParametrosGenerale> ParametrosGenerales { get; set; }

    public virtual DbSet<Prestamo> Prestamos { get; set; }

    public virtual DbSet<PrestamosAmortizacione> PrestamosAmortizaciones { get; set; }

    public virtual DbSet<Puesto> Puestos { get; set; }

    public virtual DbSet<TipoPrestamo> TipoPrestamos { get; set; }

    public virtual DbSet<TiposPago> TiposPagos { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DatosEmpresa>(entity =>
        {
            entity.HasKey(e => e.NumEmpresa).HasFillFactor(90);

            entity.ToTable("DatosEmpresa", "Prestamos");

            entity.Property(e => e.NumEmpresa).HasComment("Identidicador de la Empresa");
            entity.Property(e => e.CodPostal)
                .HasMaxLength(5)
                .IsFixedLength()
                .HasComment("Código Postal del domicilio de la empresa");
            entity.Property(e => e.Direccion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("Domicilio Completo de la Empresa");
            entity.Property(e => e.NombreCorto)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasComment("Nombre Corto de la Empresa");
            entity.Property(e => e.RazonSocial)
                .HasMaxLength(70)
                .IsUnicode(false)
                .HasComment("Razón Social de la Empresa");
            entity.Property(e => e.Rfc)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasComment("RFC / NIT")
                .HasColumnName("RFC");
            
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.RepresentanteLegal).HasMaxLength(100);
            entity.Property(e => e.SaldoActual).HasColumnType("money");
        });

        modelBuilder.Entity<Departamento>(entity =>
        {
            entity.ToTable("Departamentos", "Prestamos");

            entity.HasIndex(e => e.Descripcion, "IX_Departamentos").IsUnique();

            entity.Property(e => e.DepartamentoId)
                .HasComment("Identificador del Departamento")
                .HasColumnName("DepartamentoID");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("Descripción del Departamento");
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.ToTable("Empleados", "Prestamos");

            entity.HasIndex(e => e.Rfc, "IX_Empleados_1").IsUnique();

            entity.Property(e => e.EmpleadoId)
                .HasComment("Identidicador el Empleado")
                .HasColumnName("EmpleadoID");
            entity.Property(e => e.ApellidoMaterno)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("Apellido Materno");
            entity.Property(e => e.ApellidoPaterno)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("Apellido Paterno");
            entity.Property(e => e.Celular)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("Número del Celular");
            entity.Property(e => e.CodigoPostal).HasComment("Código Postal del Domicilio");
            entity.Property(e => e.CuentaBancaria)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasComment("Cuenta Bancaria de Depósitos");
            entity.Property(e => e.CuentaClabe)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasComment("Número CLABE de la Cuenta de Depósito");
            entity.Property(e => e.Curp)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasComment("CURP")
                .HasColumnName("CURP");
            entity.Property(e => e.DepartamentoId)
                .HasComment("ID del Departamento ")
                .HasColumnName("DepartamentoID");
            entity.Property(e => e.Domicilio)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasComment("Nombre de la calle y número");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasComment("Correo Electrónico");
            entity.Property(e => e.EmpleadoUcay).HasComment("Inidica si el empleado es directo de UCAY");
            entity.Property(e => e.EmpresaId)
                .HasComment("ID de la Empresa donde labora")
                .HasColumnName("EmpresaID");
            entity.Property(e => e.FechaIngreso)
                .HasComment("Fecha de Ingreso a la Empresa")
                .HasColumnType("datetime");
            entity.Property(e => e.ImporteCajaAhorro)
                .HasComment("Importe para la Caja de Ahorro")
                .HasColumnType("money");
            entity.Property(e => e.Nacimiento)
                .HasComment("Fecha de Nacimiento")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombres)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("Nombre(s) del Emplado");
            entity.Property(e => e.NumeroNomina).HasComment("Número del sistema de nómina del empleado");
            entity.Property(e => e.PuestoId)
                .HasComment("Puesto donde labora el empleado")
                .HasColumnName("PuestoID");
            entity.Property(e => e.Rfc)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasComment("RFC")
                .HasColumnName("RFC");
            entity.Property(e => e.SueldoLaboral)
                .HasComment("Sueldo Laboral")
                .HasColumnType("money");
            entity.Property(e => e.SueldoNominal)
                .HasComment("Sueldo Nominal")
                .HasColumnType("money");
            entity.Property(e => e.TipoPagoId)
                .HasComment("Tipo de Pago de la Nómina")
                .HasColumnName("TipoPagoID");

            entity.HasOne(d => d.Departamento).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.DepartamentoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Empleados_Departamentos");

            entity.HasOne(d => d.Puesto).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.PuestoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Empleados_Puestos");

            entity.HasOne(d => d.TipoPago).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.TipoPagoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Empleados_TiposPago");

            entity.HasOne(d => d.Empresa).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.EmpresaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Empleados_DatosEmpresa");
        });

        modelBuilder.Entity<CondicionCredito>(entity =>
        {
            entity.ToTable("CondicionesCredito", "Prestamos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MontoMaximo).HasColumnType("money");
            entity.Property(e => e.MontoMinimo).HasColumnType("money");
            entity.Property(e => e.TasaInteresMensual).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.NombreProducto).HasMaxLength(100).IsUnicode(false);

            entity.HasOne(d => d.Fondeadora).WithMany(p => p.CondicionesCredito)
                .HasForeignKey(d => d.FondeadoraId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Condiciones_DatosEmpresa");
        });

        modelBuilder.Entity<ParametrosGenerale>(entity =>
        {
            entity.HasKey(e => e.ParametrosGeneralesId);

            entity.ToTable("ParametrosGenerales", "Prestamos");

            entity.Property(e => e.ParametrosGeneralesId)
                .ValueGeneratedNever()
                .HasComment("Identificador de Parametros")
                .HasColumnName("ParametrosGeneralesID");
            entity.Property(e => e.CierreCajaAnual)
                .HasComment("Fecha de Cierre para la Caja de Ahorro")
                .HasColumnType("smalldatetime");
            entity.Property(e => e.InicioPeriodoCatorcenal)
                .HasComment("Fecha Inicial del primer periodo catorcenal del Año para el sistema de Nómina")
                .HasColumnType("smalldatetime");
            entity.Property(e => e.InicioPeriodoSemanal)
                .HasComment("Fecha Inicial del primer periodo semanal del Año para el sistema de Nómina")
                .HasColumnType("smalldatetime");
            entity.Property(e => e.TasaFijaCaja)
                .HasComment("Tasa Fija de Préstamos de la Caja de Ahorro")
                .HasColumnType("decimal(8, 2)");
            entity.Property(e => e.TasaFijaEmpresa)
                .HasComment("Tasa Fija para Préstamos de Empresa")
                .HasColumnType("decimal(8, 2)");
        });

        modelBuilder.Entity<Prestamo>(entity =>
        {
            entity.ToTable("Prestamos", "Prestamos");

            entity.Property(e => e.PrestamoId)
                .HasComment("Identificador del Préstamo")
                .HasColumnName("PrestamoID");
            entity.Property(e => e.AvalCredito).HasComment("Indica si el Préstamo cuenta con Aval");
            entity.Property(e => e.DestinoCredito)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasComment("Definición del destino del Préstamo");
            entity.Property(e => e.EmpleadoId)
                .HasComment("ID del Empleador que solicita el Préstamo")
                .HasColumnName("EmpleadoID");
            entity.Property(e => e.EmpleadoIdaval)
                .HasComment("ID del Empleado que será el Aval")
                .HasColumnName("EmpleadoIDAval");
            entity.Property(e => e.FechaAprobado)
                .HasComment("Fecha de Aporabación del Prestamo")
                .HasColumnType("smalldatetime");
            entity.Property(e => e.FechaDeposito)
                .HasComment("Fecha de Depósito del Préstamo")
                .HasColumnType("smalldatetime");
            entity.Property(e => e.FechaSolicitud)
                .HasComment("Fecha de Solicitud del Préstamo")
                .HasColumnType("smalldatetime");
            entity.Property(e => e.FinalDescuento)
                .HasComment("Fecha Final del Préstamo")
                .HasColumnType("smalldatetime");
            entity.Property(e => e.FuenteFondeoId)
                .HasComment("Fuente de Fondeo Proveedor del capital para el financiamiento")
                .HasColumnName("FuenteFondeoID");
            entity.Property(e => e.ImporteCuotas)
                .HasComment("Importe o Monto del Pago en cada Frecuencia (Pago Programado)")
                .HasColumnType("money");
            entity.Property(e => e.MontoAprobado)
                .HasComment("Importe Aprobado/Entregado")
                .HasColumnType("money");
            entity.Property(e => e.InicioDescuento)
                .HasComment("Fecha de inicio del descuento")
                .HasColumnType("smalldatetime");
            entity.Property(e => e.NumeroCuotas).HasComment("Número de Pagos o Cuotas Totales del Préstamo");
            entity.Property(e => e.PlazoMeses).HasComment("Plazo del Préstamo en Meses");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasComment("Estado del Préstamo");
            entity.Property(e => e.SueldoLaboral)
                .HasComment("Sueldo Laboral del Empleado al momento de Solicitar el Préstamo")
                .HasColumnType("money");
            entity.Property(e => e.TasaInteresAnual)
                .HasComment("Tasa de Interes Anual del Préstamo")
                .HasColumnType("decimal(8, 2)");
            entity.Property(e => e.TipoPagoId)
                .HasComment("Tipo de Pago o Amortización del Préstamo (Describe la frecuencia del Pago)")
                .HasColumnName("TipoPagoID");
            entity.Property(e => e.TipoPlazo).HasComment("Tipos de Plazo: [1]: Corto Plazo [2]: Largo Plazo [3]: Hipotecario");
            entity.Property(e => e.TipoPrestamoId)
                .HasComment("ID del Tipo de Préstamo")
                .HasColumnName("TipoPrestamoID");

            entity.HasOne(d => d.Empleado).WithMany(p => p.Prestamos)
                .HasForeignKey(d => d.EmpleadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prestamos_Empleados");

            entity.HasOne(d => d.FuenteFondeo).WithMany(p => p.PrestamosFondeados)
                .HasForeignKey(d => d.FuenteFondeoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prestamos_DatosEmpresa");

            entity.HasOne(d => d.TipoPago).WithMany(p => p.Prestamos)
                .HasForeignKey(d => d.TipoPagoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prestamos_TiposPago");

            entity.HasOne(d => d.TipoPrestamo).WithMany(p => p.Prestamos)
                .HasForeignKey(d => d.TipoPrestamoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prestamos_TipoPrestamo");
        });

        modelBuilder.Entity<PrestamosAmortizacione>(entity =>
        {
            entity.HasKey(e => e.AmortizacionId);

            entity.ToTable("PrestamosAmortizaciones", "Prestamos");

            entity.Property(e => e.AmortizacionId)
                .HasComment("Identificador de la Amortización")
                .HasColumnName("AmortizacionID");
            entity.Property(e => e.DescripcionPago)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("Descripción del Pago");
            entity.Property(e => e.DiasCalculo).HasComment("Días del Cálculo para el Interés base del Cálculo SaldoInicial");
            entity.Property(e => e.FechaPago)
                .HasComment("Fecha de Pago Programado")
                .HasColumnType("smalldatetime");
            entity.Property(e => e.ImportePago)
                .HasComment("Importe del Pago o Cuota Programada")
                .HasColumnType("money");
            entity.Property(e => e.ImportePagoCapital)
                .HasComment("Importe del Pago a Capital Programado ")
                .HasColumnType("money");
            entity.Property(e => e.ImportePagoInteres)
                .HasComment("Importe del Pago a Interés Programado ")
                .HasColumnType("money");
            entity.Property(e => e.InicioPeriodoPago)
                .HasComment("Feha de Inicio del Periodo para Nómina")
                .HasColumnType("smalldatetime");
            entity.Property(e => e.NumeroPago).HasComment("Número del Pago o Cuota Programado");
            entity.Property(e => e.PagoExtraCapital).HasColumnType("money");
            entity.Property(e => e.PrestamoId)
                .HasComment("ID del Préstamo ")
                .HasColumnName("PrestamoID");
            entity.Property(e => e.SaldoFinal)
                .HasComment("Saldo de Capital")
                .HasColumnType("money");
            entity.Property(e => e.SaldoInicial)
                .HasComment("Importe del Saldo Inicial del Préstamo")
                .HasColumnType("money");
            entity.Property(e => e.StatusPago).HasComment("Indica si el Pago ha sido aplicado");
            entity.Property(e => e.TasaInteres)
                .HasComment("Tasa de interés del cálculo")
                .HasColumnType("money");

            entity.HasOne(d => d.Prestamo).WithMany(p => p.PrestamosAmortizaciones)
                .HasForeignKey(d => d.PrestamoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PrestamosAmortizaciones_Prestamos");
        });

        modelBuilder.Entity<Puesto>(entity =>
        {
            entity.ToTable("Puestos", "Prestamos", tb => tb.HasComment("Descripción de Puestos"));

            entity.HasIndex(e => e.Descripcion, "IX_Puestos").IsUnique();

            entity.Property(e => e.PuestoId)
                .HasComment("Registo ID del Puesto")
                .HasColumnName("PuestoID");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("Descripción del Puesto");
        });

        modelBuilder.Entity<TipoPrestamo>(entity =>
        {
            entity.ToTable("TipoPrestamo", "Prestamos", tb => tb.HasComment("Desfine los Tipo de Préstamos"));

            entity.HasIndex(e => e.Descripcion, "IX_TipoPrestamo");

            entity.Property(e => e.TipoPrestamoId)
                .HasComment("Identificador del Tipo de Préstamo")
                .HasColumnName("TipoPrestamoID");
            entity.Property(e => e.Clasificacion)
                .HasConversion<int>()
                .HasComment("Clasificación del Tipo de Préstamo [1]:Corto Plazo [2]:Largo Plazo [3]:Préstamo Hipotecario [4]:Préstamo Directo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("Descripción del Tipo Préstamo ");
        });

        modelBuilder.Entity<TiposPago>(entity =>
        {
            entity.HasKey(e => e.TipoPagoId);

            entity.ToTable("TiposPago", "Prestamos");

            entity.HasIndex(e => e.Descripcion, "IX_TiposPago");

            entity.Property(e => e.TipoPagoId)
                .HasComment("Identificador del Tipo de Pago")
                .HasColumnName("TipoPagoID");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("Descripción Tipo de Pago");
            entity.Property(e => e.FrecuenciaAnual).HasComment("Níumero de Pagos por año");
            entity.Property(e => e.IntervaloAnual).HasComment("Intervalo Anual ");
            entity.Property(e => e.IntervaloDiario).HasComment("Número de Días para la frecuencia de pago");
            entity.Property(e => e.IntervaloMensual).HasComment("Número de Meses para la frecuencia del pago");
        });

        // Configuración de UsuarioSistema (Identity)
        modelBuilder.Entity<UsuarioSistema>(entity =>
        {
            entity.Property(e => e.NombreCompleto).HasMaxLength(150).IsUnicode(false);
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.UltimoAcceso).HasColumnType("datetime");

            entity.HasOne(e => e.Empleado)
                .WithMany()
                .HasForeignKey(e => e.EmpleadoId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Usuarios_Empleados");
        });

        base.OnModelCreating(modelBuilder);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
