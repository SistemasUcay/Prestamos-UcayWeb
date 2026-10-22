using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrestamosUcayNet.Domain.Entities;

namespace PrestamosUcayNet.Infrastructure.Configurations;

public class SolicitudVacacionesConfiguration : IEntityTypeConfiguration<SolicitudVacaciones>
{
    public void Configure(EntityTypeBuilder<SolicitudVacaciones> builder)
    {
        builder.ToTable("SolicitudesVacaciones", "Prestamos", tb => tb.HasComment("Registro y control de solicitudes de vacaciones por empleado"));

        builder.HasKey(e => e.SolicitudVacacionesId);

        builder.Property(e => e.SolicitudVacacionesId)
            .HasColumnName("SolicitudVacacionesID")
            .HasComment("Identificador único de la solicitud de vacaciones");

        builder.Property(e => e.EmpleadoId)
            .HasColumnName("EmpleadoID")
            .IsRequired()
            .HasComment("Identificador del Empleado solicitante");

        builder.Property(e => e.PeriodoVacacionalId)
            .HasColumnName("PeriodoVacacionalID")
            .IsRequired()
            .HasComment("Identificador del Periodo Vacacional afectado");

        builder.Property(e => e.FechaSolicitud)
            .HasColumnType("datetime2(2)")
            .IsRequired()
            .HasComment("Fecha en que se realiza la solicitud");

        builder.Property(e => e.FechaInicio)
            .HasColumnType("datetime2(2)")
            .IsRequired()
            .HasComment("Fecha de inicio de las vacaciones");

        builder.Property(e => e.FechaFin)
            .HasColumnType("datetime2(2)")
            .IsRequired()
            .HasComment("Fecha de fin de las vacaciones");

        builder.Property(e => e.FechaRegreso)
            .HasColumnType("datetime2(2)")
            .IsRequired()
            .HasComment("Fecha programada de reincorporación laboral");

        builder.Property(e => e.DiasSolicitados)
            .IsRequired()
            .HasComment("Cantidad de días disfrutados solicitados");

        builder.Property(e => e.Estado)
            .HasMaxLength(20)
            .IsUnicode(false)
            .HasDefaultValue("Pendiente")
            .IsRequired()
            .HasComment("Estado: Pendiente, Aprobada, Rechazada, Cancelada");

        builder.Property(e => e.Observaciones)
            .HasMaxLength(500)
            .IsUnicode(false)
            .HasComment("Observaciones del solicitante");

        builder.Property(e => e.FechaAprobacion)
            .HasColumnType("datetime2(2)")
            .HasComment("Fecha en la que fue resuelta");

        builder.Property(e => e.UsuarioAprobador)
            .HasMaxLength(100)
            .IsUnicode(false)
            .HasComment("Usuario que resolvió la solicitud");

        builder.Property(e => e.ObservacionesAprobacion)
            .HasMaxLength(500)
            .IsUnicode(false)
            .HasComment("Observaciones del aprobador/rechazo");

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

        // Índices de optimización
        builder.HasIndex(e => e.EmpleadoId, "IX_SolicitudesVacaciones_EmpleadoID");
        builder.HasIndex(e => e.PeriodoVacacionalId, "IX_SolicitudesVacaciones_PeriodoVacacionalID");
        builder.HasIndex(e => e.Estado, "IX_SolicitudesVacaciones_Estado");
        builder.HasIndex(e => new { e.FechaInicio, e.FechaFin }, "IX_SolicitudesVacaciones_Fechas");

        // Relaciones
        builder.HasOne(e => e.Empleado)
            .WithMany(p => p.SolicitudesVacaciones)
            .HasForeignKey(e => e.EmpleadoId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_SolicitudesVacaciones_Empleados");

        builder.HasOne(e => e.PeriodoVacacional)
            .WithMany(p => p.SolicitudesVacaciones)
            .HasForeignKey(e => e.PeriodoVacacionalId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_SolicitudesVacaciones_PeriodosVacacionales");
    }
}
