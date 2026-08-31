using System.Collections.Generic;
using System.Data.Entity;
using CarCatalogWCFService;
using CarCatalogWCFService.Models;

namespace CarCatalog.CharacterizationTests.Fakes
{
    /// <summary>
    /// <see cref="EntityModel"/> backed by in-memory sets. Database initialization is disabled and
    /// <see cref="TestDbConfiguration"/> supplies a fixed provider manifest token, so change
    /// tracking works without a database being reachable.
    /// </summary>
    public class TestEntityModel : EntityModel
    {
        public TestEntityModel(
            IEnumerable<CatalogItem> items = null,
            IEnumerable<CatalogBrand> brands = null,
            IEnumerable<CatalogType> types = null,
            IEnumerable<CatalogItemsStock> stock = null,
            IEnumerable<DiscountItem> discounts = null)
        {
            Database.SetInitializer<EntityModel>(null);

            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;

            CatalogItems = new TestDbSet<CatalogItem>(items);
            CatalogBrands = new TestDbSet<CatalogBrand>(brands);
            CatalogTypes = new TestDbSet<CatalogType>(types);
            CatalogItemsStocks = new TestDbSet<CatalogItemsStock>(stock);
            DiscountItems = new TestDbSet<DiscountItem>(discounts);
        }

        public int SaveChangesCount { get; private set; }

        public override int SaveChanges()
        {
            SaveChangesCount++;
            return 0;
        }
    }
}
