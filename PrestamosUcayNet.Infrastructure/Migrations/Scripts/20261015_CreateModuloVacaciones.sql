-- =========================================================================================
-- Script de Creación de Tablas e Índices para el Módulo de Control de Vacaciones
-- Solución: PrestamosUcayNet
-- Base de Datos: SQL Server
-- Esquema: Prestamos
-- =========================================================================================

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- 1. Tabla: PeriodosVacacionales
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Prestamos].[PeriodosVacacionales]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Prestamos].[PeriodosVacacionales] (
        [PeriodoVacacionalID]    INT IDENTITY(1,1) NOT NULL,
        [EmpleadoID]             INT NOT NULL,
        [Anio]                   INT NOT NULL,
        [AntiguedadAnios]        INT NOT NULL,
        [DiasCorrespondientes]   INT NOT NULL,
        [DiasDisfrutados]        INT NOT NULL CONSTRAINT [DF_PeriodosVacacionales_DiasDisfrutados] DEFAULT (0),
        [DiasPendientes]         INT NOT NULL,
        [FechaInicioPeriodo]     DATETIME2(2) NOT NULL,
        [FechaFinPeriodo]        DATETIME2(2) NOT NULL,
        [FechaVencimiento]       DATETIME2(2) NULL,
        [Estado]                 NVARCHAR(20) NOT NULL CONSTRAINT [DF_PeriodosVacacionales_Estado] DEFAULT ('Activo'),
        [FechaCreacion]          DATETIME2(2) NOT NULL CONSTRAINT [DF_PeriodosVacacionales_FechaCreacion] DEFAULT (SYSDATETIME()),
        [UsuarioCreacion]        NVARCHAR(100) NOT NULL,
        [FechaModificacion]      DATETIME2(2) NULL,
        [UsuarioModificacion]    NVARCHAR(100) NULL,

        CONSTRAINT [PK_PeriodosVacacionales] PRIMARY KEY CLUSTERED ([PeriodoVacacionalID] ASC),
        CONSTRAINT [FK_PeriodosVacacionales_Empleados] FOREIGN KEY ([EmpleadoID])
            REFERENCES [Prestamos].[Empleados] ([EmpleadoID])
    );

    -- Índice único compuesto para evitar duplicar el periodo del mismo año para un empleado
    CREATE UNIQUE NONCLUSTERED INDEX [IX_PeriodosVacacionales_Empleado_Anio]
        ON [Prestamos].[PeriodosVacacionales] ([EmpleadoID] ASC, [Anio] ASC);

    -- Índice cubriente para filtrados por estado y consultas de saldos
    CREATE NONCLUSTERED INDEX [IX_PeriodosVacacionales_Estado]
        ON [Prestamos].[PeriodosVacacionales] ([Estado] ASC)
        INCLUDE ([EmpleadoID], [DiasPendientes], [DiasCorrespondientes], [DiasDisfrutados]);
END
GO

-- 2. Tabla: SolicitudesVacaciones
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Prestamos].[SolicitudesVacaciones]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Prestamos].[SolicitudesVacaciones] (
        [SolicitudVacacionesID]   INT IDENTITY(1,1) NOT NULL,
        [EmpleadoID]              INT NOT NULL,
        [PeriodoVacacionalID]     INT NOT NULL,
        [FechaSolicitud]          DATETIME2(2) NOT NULL CONSTRAINT [DF_SolicitudesVacaciones_FechaSolicitud] DEFAULT (SYSDATETIME()),
        [FechaInicio]             DATETIME2(2) NOT NULL,
        [FechaFin]                DATETIME2(2) NOT NULL,
        [FechaRegreso]            DATETIME2(2) NOT NULL,
        [DiasSolicitados]         INT NOT NULL,
        [Estado]                  NVARCHAR(20) NOT NULL CONSTRAINT [DF_SolicitudesVacaciones_Estado] DEFAULT ('Pendiente'),
        [Observaciones]           NVARCHAR(500) NULL,
        [FechaAprobacion]         DATETIME2(2) NULL,
        [UsuarioAprobador]        NVARCHAR(100) NULL,
        [ObservacionesAprobacion] NVARCHAR(500) NULL,
        [FechaCreacion]           DATETIME2(2) NOT NULL CONSTRAINT [DF_SolicitudesVacaciones_FechaCreacion] DEFAULT (SYSDATETIME()),
        [UsuarioCreacion]         NVARCHAR(100) NOT NULL,
        [FechaModificacion]       DATETIME2(2) NULL,
        [UsuarioModificacion]     NVARCHAR(100) NULL,

        CONSTRAINT [PK_SolicitudesVacaciones] PRIMARY KEY CLUSTERED ([SolicitudVacacionesID] ASC),
        CONSTRAINT [FK_SolicitudesVacaciones_Empleados] FOREIGN KEY ([EmpleadoID])
            REFERENCES [Prestamos].[Empleados] ([EmpleadoID]),
        CONSTRAINT [FK_SolicitudesVacaciones_PeriodosVacacionales] FOREIGN KEY ([PeriodoVacacionalID])
            REFERENCES [Prestamos].[PeriodosVacacionales] ([PeriodoVacacionalID])
    );

    -- Índices SARGables para consultas habituales y reportes
    CREATE NONCLUSTERED INDEX [IX_SolicitudesVacaciones_EmpleadoID]
        ON [Prestamos].[SolicitudesVacaciones] ([EmpleadoID] ASC)
        INCLUDE ([Estado], [FechaInicio], [FechaFin], [DiasSolicitados]);

    CREATE NONCLUSTERED INDEX [IX_SolicitudesVacaciones_PeriodoVacacionalID]
        ON [Prestamos].[SolicitudesVacaciones] ([PeriodoVacacionalID] ASC);

    CREATE NONCLUSTERED INDEX [IX_SolicitudesVacaciones_Estado]
        ON [Prestamos].[SolicitudesVacaciones] ([Estado] ASC)
        INCLUDE ([EmpleadoID], [FechaSolicitud]);

    CREATE NONCLUSTERED INDEX [IX_SolicitudesVacaciones_Fechas]
        ON [Prestamos].[SolicitudesVacaciones] ([FechaInicio] ASC, [FechaFin] ASC);
END
GO
