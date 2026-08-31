using CarCatalog.Application;
using CarCatalog.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CarCatalog.UnitTests;

internal static class CatalogContextFixture
{
    /// <summary>
    /// A seeded context on the in-memory provider, isolated per caller.
    /// </summary>
    public static CatalogDbContext NewSeededContext()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase($"catalog-{Guid.NewGuid()}")
            .Options;

        var context = new CatalogDbContext(options);

        context.CatalogBrands.AddRange(CatalogSeedData.GetCatalogBrands());
        context.CatalogTypes.AddRange(CatalogSeedData.GetCatalogTypes());
        context.CatalogItems.AddRange(CatalogSeedData.GetCatalogItems());
        context.CatalogItemsStocks.AddRange(CatalogSeedData.GetCatalogItemsStock());
        context.DiscountItems.AddRange(CatalogSeedData.GetDiscountItems());
        context.SaveChanges();
        context.ChangeTracker.Clear();

        return context;
    }
}
