using CarCatalog.Application;
using CarCatalog.Domain;
using CarCatalog.Infrastructure;

namespace CarCatalog.UnitTests;

/// <summary>
/// The behavior every <see cref="ICatalogService"/> implementation must share. The expectations
/// come from the characterization tests written against the legacy services, so both the EF Core
/// and the in-memory implementation are held to the behavior the legacy apps had.
/// </summary>
public class CatalogServiceContractTests
{
    public static TheoryData<string> Implementations => new() { "ef", "memory" };

    private static ICatalogService Create(string implementation) => implementation switch
    {
        "ef" => new EfCatalogService(CatalogContextFixture.NewSeededContext()),
        "memory" => new InMemoryCatalogService(),
        _ => throw new ArgumentOutOfRangeException(nameof(implementation)),
    };

    [Theory]
    [MemberData(nameof(Implementations))]
    public void FindCatalogItem_PopulatesBrandAndType(string implementation)
    {
        var service = Create(implementation);

        var item = service.FindCatalogItem(1);

        Assert.NotNull(item);
        Assert.Equal("Velocari", item!.CatalogBrand?.Brand);
        Assert.Equal("Sports Car", item.CatalogType?.Type);
    }

    [Theory]
    [MemberData(nameof(Implementations))]
    public void FindCatalogItem_ReturnsNullForUnknownId(string implementation)
    {
        Assert.Null(Create(implementation).FindCatalogItem(4242));
    }

    [Theory]
    [MemberData(nameof(Implementations))]
    public void GetCatalogItemsPaginated_PagesInIdOrder(string implementation)
    {
        var service = Create(implementation);

        var page = service.GetCatalogItemsPaginated(pageSize: 5, pageIndex: 1);

        Assert.Equal(12, page.TotalItems);
        Assert.Equal(3, page.TotalPages);
        Assert.Equal(new[] { 6, 7, 8, 9, 10 }, page.Data.Select(i => i.Id));
    }

    [Theory]
    [MemberData(nameof(Implementations))]
    public void GetCatalogItems_FiltersOnBrandAndType(string implementation)
    {
        var service = Create(implementation);

        Assert.Equal(12, service.GetCatalogItems(0, 0).Count);
        Assert.Equal(new[] { 1, 2, 3, 4 }, service.GetCatalogItems(1, 0).Select(i => i.Id));
        Assert.Equal(new[] { 1, 2 }, service.GetCatalogItems(1, 1).Select(i => i.Id));
    }

    [Theory]
    [MemberData(nameof(Implementations))]
    public void CreateCatalogItem_AssignsAnIdAndStoresTheItem(string implementation)
    {
        var service = Create(implementation);

        var created = new CatalogItem
        {
            Name = "Test",
            CatalogBrandId = 1,
            CatalogTypeId = 1,
            PictureFileName = "1.png",
        };
        service.CreateCatalogItem(created);

        Assert.True(created.Id > 12);
        Assert.Equal("Test", service.FindCatalogItem(created.Id)?.Name);
    }

    [Theory]
    [MemberData(nameof(Implementations))]
    public void UpdateCatalogItem_PersistsTheNewValues(string implementation)
    {
        var service = Create(implementation);
        var item = service.FindCatalogItem(1)!;

        service.UpdateCatalogItem(new CatalogItem
        {
            Id = item.Id,
            Name = "Renamed",
            Description = item.Description,
            Price = 1234M,
            PictureFileName = item.PictureFileName,
            CatalogBrandId = item.CatalogBrandId,
            CatalogTypeId = item.CatalogTypeId,
        });

        var updated = service.FindCatalogItem(1);
        Assert.Equal("Renamed", updated?.Name);
        Assert.Equal(1234M, updated?.Price);
    }

    [Theory]
    [MemberData(nameof(Implementations))]
    public void RemoveCatalogItem_DeletesTheItem(string implementation)
    {
        var service = Create(implementation);

        service.RemoveCatalogItem(service.FindCatalogItem(1)!);

        Assert.Null(service.FindCatalogItem(1));
        Assert.Equal(11, service.GetCatalogItems(0, 0).Count);
    }

    [Theory]
    [MemberData(nameof(Implementations))]
    public void GetAvailableStock_MatchesOnDateIgnoringTime(string implementation)
    {
        var service = Create(implementation);

        Assert.Equal(100, service.GetAvailableStock(new DateTime(2017, 9, 20, 23, 45, 0), 1));
        Assert.Equal(0, service.GetAvailableStock(new DateTime(2017, 9, 19), 1));
    }

    [Theory]
    [MemberData(nameof(Implementations))]
    public void CreateAvailableStock_OverwritesTheEntryForTheSameItemAndDate(string implementation)
    {
        var service = Create(implementation);

        service.CreateAvailableStock(new CatalogItemsStock
        {
            CatalogItemId = 1,
            Date = new DateTime(2017, 9, 20),
            AvailableStock = 5,
        });

        Assert.Equal(5, service.GetAvailableStock(new DateTime(2017, 9, 20), 1));
    }

    [Theory]
    [MemberData(nameof(Implementations))]
    public void CreateAvailableStock_InsertsWhenNoEntryExistsForTheDate(string implementation)
    {
        var service = Create(implementation);

        var stock = new CatalogItemsStock
        {
            CatalogItemId = 1,
            Date = new DateTime(2017, 10, 1),
            AvailableStock = 7,
        };
        service.CreateAvailableStock(stock);

        Assert.Equal(7, stock.StockId);
        Assert.Equal(7, service.GetAvailableStock(new DateTime(2017, 10, 1), 1));
    }

    [Theory]
    [InlineData("ef", 2017, 9, 18, 0.3)]
    [InlineData("ef", 2017, 9, 21, 0.3)]
    [InlineData("ef", 2017, 9, 22, 0.25)]
    [InlineData("memory", 2017, 9, 18, 0.3)]
    [InlineData("memory", 2017, 9, 21, 0.3)]
    [InlineData("memory", 2017, 9, 22, 0.25)]
    public void GetDiscount_TreatsRangeBoundsAsInclusive(
        string implementation, int year, int month, int day, double expectedSize)
    {
        var discount = Create(implementation).GetDiscount(new DateTime(year, month, day));

        Assert.NotNull(discount);
        Assert.Equal(expectedSize, discount!.Size, 3);
    }

    [Theory]
    [MemberData(nameof(Implementations))]
    public void GetDiscount_IgnoresTheTimeOfDay(string implementation)
    {
        var discount = Create(implementation).GetDiscount(new DateTime(2017, 9, 21, 23, 59, 59));

        Assert.Equal(0.3, discount!.Size, 3);
    }

    [Theory]
    [MemberData(nameof(Implementations))]
    public void GetDiscount_ReturnsNullOutsideEveryRange(string implementation)
    {
        Assert.Null(Create(implementation).GetDiscount(new DateTime(2017, 10, 1)));
    }
}
