using System.Linq;
using CarCatalogLegacyMVC.Models;
using CarCatalogLegacyMVC.Services;
using Xunit;

namespace CarCatalog.CharacterizationTests
{
    /// <summary>
    /// Characterization tests for the MVC application's in-memory catalog service.
    /// </summary>
    public class MvcCatalogServiceMockTests
    {
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
        public void FindCatalogItem_DoesNotPopulateBrandOrTypeOnItsOwn()
        {
            var service = new CatalogServiceMock();

            var item = service.FindCatalogItem(3);

            Assert.NotNull(item);
            Assert.Null(item.CatalogBrand);
            Assert.Null(item.CatalogType);
        }

        [Fact]
        public void GetCatalogItemsPaginated_PopulatesBrandAndTypeOnTheSharedInstances()
        {
            // Composition happens in the paginated listing and mutates the stored items,
            // so a later FindCatalogItem observes the populated relations.
            var service = new CatalogServiceMock();

            var page = service.GetCatalogItemsPaginated(5, 0);

            Assert.Equal(5, page.Data.Count());
            Assert.All(page.Data, i =>
            {
                Assert.NotNull(i.CatalogBrand);
                Assert.NotNull(i.CatalogType);
            });
            Assert.NotNull(service.FindCatalogItem(3).CatalogBrand);
        }

        [Fact]
        public void GetCatalogItemsPaginated_ReportsTotalCountAndPageContents()
        {
            var service = new CatalogServiceMock();

            var page = service.GetCatalogItemsPaginated(5, 2);

            Assert.Equal(12L, page.TotalItems);
            Assert.Equal(3, page.TotalPages);
            Assert.Equal(2, page.Data.Count());
            Assert.Equal(new[] { 11, 12 }, page.Data.Select(i => i.Id).ToArray());
        }

        [Fact]
        public void RemoveCatalogItem_RemovesTheItem()
        {
            var service = new CatalogServiceMock();
            var item = service.FindCatalogItem(1);

            service.RemoveCatalogItem(item);

            Assert.Null(service.FindCatalogItem(1));
        }
    }
}
