-- Script SQL idempotente para corregir columnas faltantes
-- Error objetivo: 'StatusPago' / 'FechaPagoReal' no válidos en Prestamos.PrestamosDirectos

IF COL_LENGTH('Prestamos.PrestamosDirectos', 'StatusPago') IS NULL
BEGIN
	ALTER TABLE [Prestamos].[PrestamosDirectos]
	ADD [StatusPago] bit NOT NULL
		CONSTRAINT [DF_PrestamosDirectos_StatusPago] DEFAULT (0);
END
GO

IF COL_LENGTH('Prestamos.PrestamosDirectos', 'FechaPagoReal') IS NULL
BEGIN
	ALTER TABLE [Prestamos].[PrestamosDirectos]
	ADD [FechaPagoReal] datetime2 NULL;
END
GO
