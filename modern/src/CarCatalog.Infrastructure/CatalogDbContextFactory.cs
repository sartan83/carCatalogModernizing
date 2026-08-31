using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CarCatalog.Infrastructure;

/// <summary>
/// Used by the EF Core tools only; migrations are generated against SQL Server without a
/// reachable server, so the connection string is a placeholder.
/// </summary>
public class CatalogDbContextFactory : IDesignTimeDbContextFactory<CatalogDbContext>
{
    public CatalogDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseSqlServer("Server=(design-time);Database=CarCatalog;Trusted_Connection=True")
            .Options;

        return new CatalogDbContext(options);
    }
}
