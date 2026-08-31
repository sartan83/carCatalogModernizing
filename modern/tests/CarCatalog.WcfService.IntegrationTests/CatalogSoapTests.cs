using CarCatalog.Domain;

namespace CarCatalog.WcfService.IntegrationTests;

public class CatalogSoapTests(CatalogServiceFixture fixture) : IClassFixture<CatalogServiceFixture>
{
    [Fact]
    public void GetCatalogItems_WithoutFilters_ReturnsTheSeededCatalog()
    {
        var items = fixture.CreateChannel().GetCatalogItems(0, 0);

        // The host is shared with the tests that add items, hence the lower bound.
        Assert.True(items.Count >= 12, $"Expected at least the 12 seeded items, got {items.Count}.");
        Assert.Contains(items, item => item.Name == "Velocari Strada SV");
    }

    [Fact]
    public void GetCatalogItems_FiltersByBrandAndType()
    {
        var channel = fixture.CreateChannel();

        var byBrand = channel.GetCatalogItems(1, 0);
        var byBrandAndType = channel.GetCatalogItems(1, 1);

        Assert.All(byBrand, item => Assert.Equal(1, item.CatalogBrandId));
        Assert.All(byBrandAndType, item =>
        {
            Assert.Equal(1, item.CatalogBrandId);
            Assert.Equal(1, item.CatalogTypeId);
        });
    }

    [Fact]
    public void FindCatalogItem_PopulatesBrandAndType()
    {
        var item = fixture.CreateChannel().FindCatalogItem(1);

        Assert.NotNull(item);
        Assert.NotNull(item!.CatalogBrand);
        Assert.NotNull(item.CatalogType);
    }

    [Fact]
    public void GetCatalogBrandsAndTypes_ReturnTheLookups()
    {
        var channel = fixture.CreateChannel();

        Assert.NotEmpty(channel.GetCatalogBrands());
        Assert.NotEmpty(channel.GetCatalogTypes());
    }

    [Fact]
    public void CreateAvailableStock_IsVisibleToGetAvailableStock()
    {
        var channel = fixture.CreateChannel();
        var date = new DateTime(2030, 1, 15);

        channel.CreateAvailableStock(new CatalogItemsStock { CatalogItemId = 3, Date = date, AvailableStock = 7 });

        Assert.Equal(7, channel.GetAvailableStock(date, 3));
        Assert.Equal(0, channel.GetAvailableStock(date.AddDays(1), 3));
    }

    [Fact]
    public void GetDiscount_ReturnsNullOutsideAnyDiscountRange()
    {
        Assert.Null(fixture.CreateChannel().GetDiscount(new DateTime(1990, 1, 1)));
    }

    [Fact]
    public void CreateCatalogItem_AddsTheItemToTheCatalog()
    {
        var channel = fixture.CreateChannel();

        channel.CreateCatalogItem(new CatalogItem
        {
            Name = "SOAP test car",
            Price = 1234,
            CatalogBrandId = 1,
            CatalogTypeId = 1,
            PictureFileName = "1.png",
        });

        Assert.Contains(channel.GetCatalogItems(0, 0), item => item.Name == "SOAP test car");
    }
}
