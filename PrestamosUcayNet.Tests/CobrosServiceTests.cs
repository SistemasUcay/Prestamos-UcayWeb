using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PrestamosUcayNet.Application.Cobros.Commands;
using PrestamosUcayNet.Domain.Entities;
using PrestamosUcayNet.Infrastructure.Data;
using PrestamosUcayNet.Infrastructure.Services;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;

namespace PrestamosUcayNet.Tests;

public class CobrosServiceTests
{
    private UcayNetDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<UcayNetDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new UcayNetDbContext(options);
    }

    private void SeedData(UcayNetDbContext context, int prestamoId = 1)
    {
        // Insertar Fondeadora y Empleado para dependencias
        var fondeadora = new DatosEmpresa { NumEmpresa = 1, RazonSocial = "Fondeadora Test", Rfc = "FT123" };
        var empresa = new DatosEmpresa { NumEmpresa = 2, RazonSocial = "Empresa Test", Rfc = "ET123" };
        var empleado = new Empleado { EmpleadoId = 1, Nombres = "Juan", ApellidoPaterno = "Perez", EmpresaId = 2, Domicilio = "Calle Falsa 123", Rfc = "PEJU800101" };
        var tipoPago = new TiposPago { TipoPagoId = 1, Descripcion = "Quincenal" };
        var tipoPrestamo = new TipoPrestamo { TipoPrestamoId = 1, Descripcion = "Nómina" };

        context.DatosEmpresas.AddRange(fondeadora, empresa);
        context.Empleados.Add(empleado);
        context.TiposPagos.Add(tipoPago);
        context.TipoPrestamos.Add(tipoPrestamo);

        var prestamo = new Prestamo
        {
            PrestamoId = prestamoId,
            EmpleadoId = 1,
            FuenteFondeoId = 1,
            TipoPrestamoId = 1,
            TipoPagoId = 1,
            MontoAprobado = 10000,
            SaldoPendiente = 10000,
            TotalPagar = 11000,
            Estado = "A",
            DestinoCredito = "Test",
            InicioDescuento = DateTime.Today,
            FinalDescuento = DateTime.Today.AddMonths(3)
        };

        var amortizaciones = new List<PrestamosAmortizacione>
        {
            new PrestamosAmortizacione
            {
                AmortizacionId = 1,
                PrestamoId = prestamoId,
                NumeroPago = 1,
                FechaPago = DateTime.Today.AddDays(15),
                InicioPeriodoPago = DateTime.Today,
                DescripcionPago = "Cuota 1",
                ImportePago = 1000,
                ImportePagoCapital = 800,
                ImportePagoInteres = 200,
                SaldoInicial = 10000,
                SaldoFinal = 9200,
                StatusPago = false
            },
            new PrestamosAmortizacione
            {
                AmortizacionId = 2,
                PrestamoId = prestamoId,
                NumeroPago = 2,
                FechaPago = DateTime.Today.AddDays(30),
                InicioPeriodoPago = DateTime.Today.AddDays(15),
                DescripcionPago = "Cuota 2",
                ImportePago = 1000,
                ImportePagoCapital = 800,
                ImportePagoInteres = 200,
                SaldoInicial = 9200,
                SaldoFinal = 8400,
                StatusPago = false
            }
        };

        var prestamoDirecto = new PrestamoDirecto
        {
            NumPrestamoDirecto = 1,
            PrestamoId = prestamoId,
            Descripcion = "Aguinaldo",
            MontoSolicitado = 10000,
            TasaMensual = 0.006m,
            FechaPago = DateTime.Today.AddDays(15),
            DiazPlazo = 15,
            MontoIntereses = 60,
            TotalCapitalInteres = 10060,
            StatusPago = false
        };

        context.Prestamos.Add(prestamo);
        context.PrestamosAmortizaciones.AddRange(amortizaciones);
        context.PrestamosDirectos.Add(prestamoDirecto);
        context.SaveChanges();
    }

    [Fact]
    public async Task AplicarPagoIndividual_Exacto_DebeMarcarComoPagadoYReducirSaldo()
    {
        // Arrange
        var context = GetDbContext();
        SeedData(context, 1);
        var service = new CobrosService(context);

        var command = new AplicarPagoIndividualCommand
        {
            PrestamoId = 1,
            NumeroPago = 1,
            MontoRecibido = 1000,
            MetodoPago = "Efectivo",
            Referencia = "Ref-1"
        };

        // Act
        var result = await service.AplicarPagoIndividualAsync(command, "user-test");

        // Assert
        Assert.True(result);

        var amort = await context.PrestamosAmortizaciones.FindAsync(1);
        Assert.NotNull(amort);
        Assert.True(amort.StatusPago);
        Assert.Equal(0, amort.PagoExtraCapital);
        Assert.Equal(9200, amort.SaldoFinal);

        var prestamo = await context.Prestamos.FindAsync(1);
        Assert.Equal(9200, prestamo.SaldoPendiente);
    }

    [Fact]
    public async Task AplicarPagoIndividual_Parcial_DebeReducirImportePendienteYNoMarcarComoPagado()
    {
        // Arrange
        var context = GetDbContext();
        SeedData(context, 2);
        var service = new CobrosService(context);

        var command = new AplicarPagoIndividualCommand
        {
            PrestamoId = 2,
            NumeroPago = 1,
            MontoRecibido = 500, // Menor que 1000
            MetodoPago = "Transferencia"
        };

        // Act
        var result = await service.AplicarPagoIndividualAsync(command, "user-test");

        // Assert
        Assert.True(result);

        var amort = await context.PrestamosAmortizaciones.FindAsync(1);
        Assert.NotNull(amort);
        Assert.False(amort.StatusPago);
        // Interés cubierto = 200, capital cubierto = 300. 
        // Importe restante = 1000 - 500 = 500. Importe capital restante = 800 - 300 = 500.
        Assert.Equal(500, amort.ImportePago);
        Assert.Equal(500, amort.ImportePagoCapital);
        Assert.Equal(0, amort.ImportePagoInteres); 

        var prestamo = await context.Prestamos.FindAsync(2);
        Assert.Equal(9700, prestamo.SaldoPendiente); // 10000 - 300 capital = 9700
    }

    [Fact]
    public async Task AplicarPagoIndividual_Exceso_DebePagarCuotaYAplicarRemanenteAlCapital()
    {
        // Arrange
        var context = GetDbContext();
        SeedData(context, 3);
        var service = new CobrosService(context);

        var command = new AplicarPagoIndividualCommand
        {
            PrestamoId = 3,
            NumeroPago = 1,
            MontoRecibido = 1500, // 500 excedente
            MetodoPago = "Efectivo"
        };

        // Act
        var result = await service.AplicarPagoIndividualAsync(command, "user-test");

        // Assert
        Assert.True(result);

        var amort = await context.PrestamosAmortizaciones.FindAsync(1);
        Assert.NotNull(amort);
        Assert.True(amort.StatusPago);
        Assert.Equal(500, amort.PagoExtraCapital);
        // Reducción capital total: 800 (programado) + 500 (extra) = 1300.
        // Saldo final: 10000 - 1300 = 8700.
        Assert.Equal(8700, amort.SaldoFinal);

        var prestamo = await context.Prestamos.FindAsync(3);
        Assert.Equal(8700, prestamo.SaldoPendiente);

        // Validar propagación a la cuota 2
        var amort2 = await context.PrestamosAmortizaciones.FindAsync(2);
        Assert.NotNull(amort2);
        Assert.Equal(8700, amort2.SaldoInicial);
        Assert.Equal(7900, amort2.SaldoFinal); // 8400 anterior - 500 reducción = 7900
    }

    [Fact]
    public async Task AplicarPagoIndividual_FueraDeSecuencia_DebeLanzarExcepcion()
    {
        // Arrange
        var context = GetDbContext();
        SeedData(context, 4);
        var service = new CobrosService(context);

        var command = new AplicarPagoIndividualCommand
        {
            PrestamoId = 4,
            NumeroPago = 2, // Intentando pagar la cuota 2 antes que la 1
            MontoRecibido = 1000,
            MetodoPago = "Efectivo"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            service.AplicarPagoIndividualAsync(command, "user-test")
        );
    }

    [Fact]
    public async Task AplicarPagoMasivo_PrestamoDirecto_Exacto_DebeMarcarComoPagado()
    {
        var context = GetDbContext();
        SeedData(context, 5);
        var service = new CobrosService(context);

        var command = new AplicarPagoMasivoCommand
        {
            OrigenNomina = "Lote Directo Test",
            MetodoPago = "Efectivo",
            FechaCobro = DateTime.Today,
            Detalles = new List<PrestamosUcayNet.Application.DTOs.DetallePagoMasivoDto>
            {
                new()
                {
                    PrestamoId = 5,
                    EsPrestamoDirecto = true,
                    PrestamoDirectoId = 1,
                    MontoAplicado = 10060
                }
            }
        };

        var result = await service.AplicarPagoMasivoAsync(command, "user-test");

        Assert.Equal(1, result.Exitosos);
        Assert.Equal(0, result.Fallidos);
        Assert.Empty(result.DetallesErrores);

        var directo = await context.PrestamosDirectos.FindAsync(1);
        Assert.NotNull(directo);
        Assert.True(directo.StatusPago);

        var detalle = await context.PrestamoDetallePagos.FirstOrDefaultAsync(x => x.PrestamoId == 5 && x.PrestamoDirectoId == 1);
        Assert.NotNull(detalle);
    }
}
