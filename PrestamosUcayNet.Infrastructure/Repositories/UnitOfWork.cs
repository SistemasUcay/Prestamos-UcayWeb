using System;
using System.Threading.Tasks;
using PrestamosUcayNet.Domain.Interfaces;
using PrestamosUcayNet.Infrastructure.Data;

namespace PrestamosUcayNet.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly UcayNetDbContext _context;

    public UnitOfWork(UcayNetDbContext context)
    {
        _context = context;
    }

    public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
