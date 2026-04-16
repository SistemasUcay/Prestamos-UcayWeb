using System;
using System.Threading.Tasks;

namespace PrestamosUcayNet.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}
