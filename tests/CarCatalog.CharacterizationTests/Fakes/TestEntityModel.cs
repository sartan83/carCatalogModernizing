using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using CarCatalogWCFService;
using CarCatalogWCFService.Models;

namespace CarCatalog.CharacterizationTests.Fakes
{
    /// <summary>
    /// Raised instead of returning a <see cref="DbEntityEntry"/>, which cannot be created
    /// outside of an initialized EF context. Tests catch it to observe the state of the
    /// entities at the point the production code marks an entity as modified.
    /// </summary>
    public class EntryTrackedException : Exception
    {
        public EntryTrackedException(object entity)
        {
            Entity = entity;
        }

        public object Entity { get; private set; }
    }

    /// <summary>
    /// <see cref="EntityModel"/> backed by in-memory sets. The context is never initialized,
    /// so no database connection is opened.
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

        public override DbEntityEntry Entry(object entity)
        {
            throw new EntryTrackedException(entity);
        }

        public override DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity)
        {
            throw new EntryTrackedException(entity);
        }
    }
}
