using CarCatalog.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarCatalog.Infrastructure;

/// <summary>
/// Applies migrations and seeds the preconfigured catalog, replacing the legacy
/// <c>CatalogDBInitializer</c> classes that ran through EF6 database initializers.
/// </summary>
public class CatalogDbInitializer
{
    private readonly CatalogDbContext db;
    private readonly ILogger<CatalogDbInitializer> logger;

    public CatalogDbInitializer(CatalogDbContext db, ILogger<CatalogDbInitializer> logger)
    {
        this.db = db;
        this.logger = logger;
    }

    public void Initialize()
    {
        db.Database.Migrate();
        Seed();
    }

    public void Seed()
    {
        if (!db.CatalogBrands.Any())
        {
            db.CatalogBrands.AddRange(CatalogSeedData.GetCatalogBrands());
        }

        if (!db.CatalogTypes.Any())
        {
            db.CatalogTypes.AddRange(CatalogSeedData.GetCatalogTypes());
        }

        db.SaveChanges();

        if (!db.CatalogItems.Any())
        {
            db.CatalogItems.AddRange(CatalogSeedData.GetCatalogItems());
        }

        if (!db.CatalogItemsStocks.Any())
        {
            db.CatalogItemsStocks.AddRange(CatalogSeedData.GetCatalogItemsStock());
        }

        if (!db.DiscountItems.Any())
        {
            db.DiscountItems.AddRange(CatalogSeedData.GetDiscountItems());
        }

        var seeded = db.SaveChanges();
        logger.LogInformation("Catalog database ready, {Count} seed rows written.", seeded);
    }
}
