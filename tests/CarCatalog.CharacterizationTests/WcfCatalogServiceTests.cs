using System;
using System.Collections.Generic;
using System.Linq;
using CarCatalog.CharacterizationTests.Fakes;
using CarCatalogWCFService;
using CarCatalogWCFService.Models;
using CarCatalogWCFService.Models.Infrastructure;
using Xunit;

namespace CarCatalog.CharacterizationTests
{
    /// <summary>
    /// Characterization tests for <see cref="CatalogService"/> (the EF-backed WCF
    /// implementation). They document today's behavior, including its quirks.
    /// </summary>
    public class WcfCatalogServiceTests
    {
        private static TestEntityModel NewContext()
        {
            return new TestEntityModel(
                PreconfiguredData.GetPreconfiguredCatalogItems(),
                PreconfiguredData.GetPreconfiguredCatalogBrands(),
                PreconfiguredData.GetPreconfiguredCatalogTypes(),
                PreconfiguredData.GetPreconfiguredCatalogItemsStock(),
                PreconfiguredData.GetPreconfiguredDiscountItems());
        }

        [Theory]
        [InlineData(2017, 9, 18, 0.3)]   // first day of the range
        [InlineData(2017, 9, 20, 0.3)]   // inside the range
        [InlineData(2017, 9, 21, 0.3)]   // last day of the range
        [InlineData(2017, 9, 22, 0.25)]  // first day of the next range
        public void GetDiscount_ReturnsDiscountWhoseRangeContainsTheDay(int year, int month, int day, double expectedSize)
        {
            using (var context = NewContext())
            {
                var service = new CatalogService(context);

                var discount = service.GetDiscount(new DateTime(year, month, day));

                Assert.NotNull(discount);
                Assert.Equal(expectedSize, discount.Size, 3);
            }
        }

        [Fact]
        public void GetDiscount_IgnoresTimeOfDay()
        {
            using (var context = NewContext())
            {
                var service = new CatalogService(context);

                var discount = service.GetDiscount(new DateTime(2017, 9, 21, 23, 59, 59));

                Assert.NotNull(discount);
                Assert.Equal(0.3, discount.Size, 3);
            }
        }

        [Fact]
        public void GetDiscount_ReturnsNullOutsideEveryRange()
        {
            using (var context = NewContext())
            {
                var service = new CatalogService(context);

                Assert.Null(service.GetDiscount(new DateTime(2017, 10, 1)));
            }
        }

        [Fact]
        public void FindCatalogItem_PopulatesBrandAndType()
        {
            using (var context = NewContext())
            {
                var service = new CatalogService(context);

                var item = service.FindCatalogItem(3);

                Assert.NotNull(item);
                Assert.NotNull(item.CatalogBrand);
                Assert.NotNull(item.CatalogType);
                Assert.Equal("Velocari", item.CatalogBrand.Brand);
                Assert.Equal("GT", item.CatalogType.Type);
            }
        }

        [Fact]
        public void FindCatalogItem_ReturnsNullForUnknownId()
        {
            using (var context = NewContext())
            {
                var service = new CatalogService(context);

                Assert.Null(service.FindCatalogItem(int.MaxValue));
            }
        }

        [Fact]
        public void CreateCatalogItem_AssignsMaxIdPlusOne()
        {
            using (var context = NewContext())
            {
                var service = new CatalogService(context);
                var newItem = new CatalogItem { Name = "Test", CatalogBrandId = 1, CatalogTypeId = 1 };

                service.CreateCatalogItem(newItem);

                Assert.Equal(13, newItem.Id);
                Assert.Contains(newItem, context.CatalogItems);
                Assert.Equal(1, context.SaveChangesCount);
            }
        }

        [Fact]
        public void CreateAvailableStock_InsertsNewEntryWhenNoneExistsForItemAndDate()
        {
            using (var context = NewContext())
            {
                var service = new CatalogService(context);
                var stock = new CatalogItemsStock
                {
                    CatalogItemId = 1,
                    Date = new DateTime(2017, 10, 1),
                    AvailableStock = 7
                };

                service.CreateAvailableStock(stock);

                Assert.Equal(7, stock.StockId);
                Assert.Equal(7, context.CatalogItemsStocks.Count());
                Assert.Equal(1, context.SaveChangesCount);
            }
        }

        [Fact]
        public void CreateAvailableStock_OverwritesExistingEntryForSameItemAndDate()
        {
            using (var context = NewContext())
            {
                var service = new CatalogService(context);
                var existing = context.CatalogItemsStocks.Single(s => s.StockId == 1);

                // The overwrite path marks the entity as Modified through DbContext.Entry,
                // which the in-memory context cannot produce, so it signals the call instead.
                var tracked = Assert.Throws<EntryTrackedException>(() =>
                    service.CreateAvailableStock(new CatalogItemsStock
                    {
                        CatalogItemId = existing.CatalogItemId,
                        Date = existing.Date,
                        AvailableStock = 5
                    }));

                Assert.Same(existing, tracked.Entity);
                Assert.Equal(5, existing.AvailableStock);
                Assert.Equal(6, context.CatalogItemsStocks.Count());
            }
        }

        [Fact]
        public void GetAvailableStock_MatchesOnDateIgnoringTime()
        {
            using (var context = NewContext())
            {
                var service = new CatalogService(context);

                Assert.Equal(120, service.GetAvailableStock(new DateTime(2017, 9, 21, 13, 0, 0), 1));
            }
        }

        [Fact]
        public void GetCatalogItems_TreatsZeroFiltersAsNoFilter()
        {
            using (var context = NewContext())
            {
                var service = new CatalogService(context);

                Assert.Equal(12, service.GetCatalogItems(0, 0).Count);
                Assert.Equal(4, service.GetCatalogItems(1, 0).Count);
                Assert.Equal(2, service.GetCatalogItems(1, 1).Count);
            }
        }

        [Fact]
        public void RemoveCatalogItem_RemovesAndSaves()
        {
            using (var context = NewContext())
            {
                var service = new CatalogService(context);
                var item = context.CatalogItems.Single(i => i.Id == 1);

                service.RemoveCatalogItem(item);

                Assert.Equal(11, context.CatalogItems.Count());
                Assert.Equal(1, context.SaveChangesCount);
            }
        }

        [Fact]
        public void GetCatalogBrandsAndTypes_ReturnEveryRow()
        {
            using (var context = NewContext())
            {
                var service = new CatalogService(context);

                Assert.Equal(5, service.GetCatalogBrands().Count);
                Assert.Equal(4, service.GetCatalogTypes().Count);
            }
        }
    }
}
