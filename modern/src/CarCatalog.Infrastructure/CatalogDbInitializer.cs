using CarCatalog.Application;
using CarCatalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarCatalog.Infrastructure;

/// <summary>
/// Applies migrations and seeds the preconfigured catalog, replacing the legacy
/// <c>CatalogDBInitializer</c> classes that ran through EF6 database initializers.
/// </summary>
public class CatalogDbInitializer
{
    private const int Attempts = 10;
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(5);

    private readonly CatalogDbContext db;
    private readonly ILogger<CatalogDbInitializer> logger;

    public CatalogDbInitializer(CatalogDbContext db, ILogger<CatalogDbInitializer> logger)
    {
        this.db = db;
        this.logger = logger;
    }

    /// <summary>
    /// Retries because several apps share one database: in containers they start together and race
    /// each other on creating the database, applying migrations and seeding.
    /// </summary>
    public void Initialize()
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                db.Database.Migrate();
                Seed();
                return;
            }
            catch (Exception exception) when (attempt < Attempts)
            {
                logger.LogWarning(
                    exception,
                    "Catalog database not ready on attempt {Attempt} of {Attempts}, retrying in {Delay}.",
                    attempt,
                    Attempts,
                    RetryDelay);

                db.ChangeTracker.Clear();
                Thread.Sleep(RetryDelay);
            }
        }
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
            // DiscountItem.Id is an identity column in the legacy schema, so the ids of the seed rows
            // are assigned by the database, exactly as they were under the EF6 initializer.
            db.DiscountItems.AddRange(CatalogSeedData.GetDiscountItems()
                .Select(discount => new DiscountItem
                {
                    Start = discount.Start,
                    End = discount.End,
                    Size = discount.Size,
                }));
        }

        var seeded = db.SaveChanges();
        logger.LogInformation("Catalog database ready, {Count} seed rows written.", seeded);
    }
}
