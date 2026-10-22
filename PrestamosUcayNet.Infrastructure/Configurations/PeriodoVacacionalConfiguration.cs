using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrestamosUcayNet.Domain.Entities;

namespace PrestamosUcayNet.Infrastructure.Configurations;

public class PeriodoVacacionalConfiguration : IEntityTypeConfiguration<PeriodoVacacional>
{
    public void Configure(EntityTypeBuilder<PeriodoVacacional> builder)
    {
        builder.ToTable("PeriodosVacacionales", "Prestamos", tb => tb.HasComment("Control de periodos y saldos de vacaciones por empleado y año"));

        builder.HasKey(e => e.PeriodoVacacionalId);

        builder.Property(e => e.PeriodoVacacionalId)
            .HasColumnName("PeriodoVacacionalID")
            .HasComment("Identificador único del Periodo Vacacional");

        builder.Property(e => e.EmpleadoId)
            .HasColumnName("EmpleadoID")
            .IsRequired()
            .HasComment("Identificador del Empleado titular");

        builder.Property(e => e.Anio)
            .IsRequired()
            .HasComment("Año del periodo vacacional");

        builder.Property(e => e.AntiguedadAnios)
            .IsRequired()
            .HasComment("Años de servicio cumplidos");

        builder.Property(e => e.DiasCorrespondientes)
            .IsRequired()
            .HasComment("Días de vacaciones correspondientes en el periodo");

        builder.Property(e => e.DiasDisfrutados)
            .IsRequired()
            .HasDefaultValue(0)
            .HasComment("Días disfrutados acumulados en el periodo");

        builder.Property(e => e.DiasPendientes)
            .IsRequired()
            .HasComment("Días pendientes por disfrutar");

        builder.Property(e => e.FechaInicioPeriodo)
            .HasColumnType("datetime2(2)")
            .IsRequired()
            .HasComment("Fecha inicio del ciclo anual");

        builder.Property(e => e.FechaFinPeriodo)
            .HasColumnType("datetime2(2)")
            .IsRequired()
            .HasComment("Fecha fin del ciclo anual");

        builder.Property(e => e.FechaVencimiento)
            .HasColumnType("datetime2(2)")
            .HasComment("Fecha límite para gozar las vacaciones");

        builder.Property(e => e.Estado)
            .HasMaxLength(20)
            .IsUnicode(false)
            .HasDefaultValue("Activo")
            .IsRequired()
            .HasComment("Estado del periodo: Activo, Cerrado, Vencido");

        // Auditoría
        builder.Property(e => e.FechaCreacion)
            .HasColumnType("datetime2(2)")
            .IsRequired();

        builder.Property(e => e.UsuarioCreacion)
            .HasMaxLength(100)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(e => e.FechaModificacion)
            .HasColumnType("datetime2(2)");

        builder.Property(e => e.UsuarioModificacion)
            .HasMaxLength(100)
            .IsUnicode(false);

        // Índices SARGables y Clave Única por Empleado y Año
        builder.HasIndex(e => new { e.EmpleadoId, e.Anio }, "IX_PeriodosVacacionales_Empleado_Anio")
            .IsUnique();

        builder.HasIndex(e => e.Estado, "IX_PeriodosVacacionales_Estado");

        // Relación con Empleado
        builder.HasOne(e => e.Empleado)
            .WithMany(p => p.PeriodosVacacionales)
            .HasForeignKey(e => e.EmpleadoId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PeriodosVacacionales_Empleados");
    }
}
