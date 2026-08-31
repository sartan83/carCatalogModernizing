using System;
using System.Linq;
using CarCatalogWCFService;
using CarCatalogWCFService.Models;
using Xunit;

namespace CarCatalog.CharacterizationTests
{
    /// <summary>
    /// Characterization tests for <see cref="CatalogServiceMock"/>, the in-memory
    /// implementation used when <c>UseMockData</c> is enabled.
    /// </summary>
    public class WcfCatalogServiceMockTests
    {
        [Fact]
        public void GetDiscount_IsNotImplemented()
        {
            // Documented gap: the mock has never implemented discounts. Left as-is in this phase.
            var service = new CatalogServiceMock();

            Assert.Throws<NotImplementedException>(() => service.GetDiscount(new DateTime(2017, 9, 20)));
        }

        [Fact]
        public void FindCatalogItem_DoesNotPopulateBrandOrType()
        {
            // Unlike the EF-backed service, the mock returns the raw item.
            var service = new CatalogServiceMock();

            var item = service.FindCatalogItem(3);

            Assert.NotNull(item);
            Assert.Null(item.CatalogBrand);
            Assert.Null(item.CatalogType);
        }

        [Fact]
        public void CreateCatalogItem_AssignsMaxIdPlusOne()
        {
            var service = new CatalogServiceMock();
            var newItem = new CatalogItem { Name = "Test", CatalogBrandId = 1, CatalogTypeId = 1 };

            service.CreateCatalogItem(newItem);

            Assert.Equal(13, newItem.Id);
            Assert.Same(newItem, service.FindCatalogItem(13));
        }

        [Fact]
        public void CreateAvailableStock_OverwritesExistingEntryForSameItemAndDate()
        {
            var service = new CatalogServiceMock();

            service.CreateAvailableStock(new CatalogItemsStock
            {
                CatalogItemId = 1,
                Date = new DateTime(2017, 9, 20),
                AvailableStock = 5
            });

            Assert.Equal(5, service.GetAvailableStock(new DateTime(2017, 9, 20), 1));
            Assert.Equal(120, service.GetAvailableStock(new DateTime(2017, 9, 21), 1));
        }

        [Fact]
        public void CreateAvailableStock_InsertsWithMaxIdPlusOneForNewDate()
        {
            var service = new CatalogServiceMock();
            var stock = new CatalogItemsStock
            {
                CatalogItemId = 1,
                Date = new DateTime(2017, 10, 1),
                AvailableStock = 7
            };

            service.CreateAvailableStock(stock);

            Assert.Equal(7, stock.StockId);
            Assert.Equal(7, service.GetAvailableStock(new DateTime(2017, 10, 1), 1));
        }

        [Fact]
        public void UpdateCatalogItem_ReplacesTheStoredInstance()
        {
            var service = new CatalogServiceMock();
            var modified = new CatalogItem { Id = 2, Name = "Renamed", CatalogBrandId = 1, CatalogTypeId = 1 };

            service.UpdateCatalogItem(modified);

            Assert.Same(modified, service.FindCatalogItem(2));
        }

        [Fact]
        public void GetCatalogItems_TreatsZeroFiltersAsNoFilter()
        {
            var service = new CatalogServiceMock();

            Assert.Equal(12, service.GetCatalogItems(0, 0).Count);
            Assert.Equal(4, service.GetCatalogItems(1, 0).Count);
            Assert.Equal(2, service.GetCatalogItems(1, 1).Count);
        }
    }
}
