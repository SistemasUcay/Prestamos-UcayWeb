using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PrestamosUcayNet.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<UcayNetDbContext>
{
    public UcayNetDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<UcayNetDbContext>();
        builder.UseSqlServer("Server=localhost;Database=UcayNetDB;Trusted_Connection=True;Encrypt=False;");

        return new UcayNetDbContext(builder.Options);
    }
}
