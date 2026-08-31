using Catalog.Domain.Entities;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Catalog.Application.Tests;

/// <summary>
/// Characterization tests for the catalog service behavior inherited from the legacy
/// MVC/WebForms services and the WCF service.
/// </summary>
public class CatalogServiceTests : IDisposable
{
    private readonly CatalogDbContext _db;
    private readonly CatalogService _service;

    public CatalogServiceTests()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new CatalogDbContext(options);
        _service = new CatalogService(_db);

        Seed();
    }

    private void Seed()
    {
        var brands = new[]
        {
            new CatalogBrand { Id = 1, Brand = "Velocari" },
            new CatalogBrand { Id = 2, Brand = "Nordwerk" }
        };

        var types = new[]
        {
            new CatalogType { Id = 1, Type = "Sports car" },
            new CatalogType { Id = 2, Type = "SUV" }
        };

        var items = new[]
        {
            NewItem(1, "Velocari GT", 120000m, 1, 1),
            NewItem(2, "Velocari Sprint", 90000m, 1, 2),
            NewItem(3, "Nordwerk Trail", 75000m, 2, 2)
        };

        _db.CatalogBrands.AddRange(brands);
        _db.CatalogTypes.AddRange(types);
        _db.CatalogItems.AddRange(items);
        _db.SaveChanges();

        foreach (var entry in _db.ChangeTracker.Entries().ToList())
        {
            entry.State = EntityState.Detached;
        }
    }

    private static CatalogItem NewItem(int id, string name, decimal price, int brandId, int typeId) => new()
    {
        Id = id,
        Name = name,
        Description = name,
        Price = price,
        CatalogBrandId = brandId,
        CatalogTypeId = typeId,
        AvailableStock = 10
    };

    [Fact]
    public void GetCatalogItemsPaginated_ReturnsRequestedPageWithTotals()
    {
        var page = _service.GetCatalogItemsPaginated(pageSize: 2, pageIndex: 1);

        Assert.Equal(1, page.ActualPage);
        Assert.Equal(2, page.ItemsPerPage);
        Assert.Equal(3, page.TotalItems);
        Assert.Equal(2, page.TotalPages);
        var item = Assert.Single(page.Data);
        Assert.Equal(3, item.Id);
    }

    [Fact]
    public void GetCatalogItemsPaginated_IncludesBrandAndType()
    {
        var page = _service.GetCatalogItemsPaginated(pageSize: 10, pageIndex: 0);

        Assert.All(page.Data, item =>
        {
            Assert.NotNull(item.CatalogBrand);
            Assert.NotNull(item.CatalogType);
        });
    }

    [Fact]
    public void GetCatalogItems_WithoutFilters_ReturnsEverything()
    {
        var items = _service.GetCatalogItems(brandIdFilter: 0, typeIdFilter: 0);

        Assert.Equal(3, items.Count);
    }

    [Fact]
    public void GetCatalogItems_FiltersByBrandAndType()
    {
        Assert.Equal(new[] { 1, 2 }, _service.GetCatalogItems(1, 0).Select(i => i.Id));
        Assert.Equal(new[] { 2, 3 }, _service.GetCatalogItems(0, 2).Select(i => i.Id));
        Assert.Equal(new[] { 2 }, _service.GetCatalogItems(1, 2).Select(i => i.Id));
    }

    [Fact]
    public void FindCatalogItem_ReturnsItemWithRelations_OrNullWhenMissing()
    {
        var found = _service.FindCatalogItem(1);

        Assert.NotNull(found);
        Assert.Equal("Velocari GT", found!.Name);
        Assert.Equal("Velocari", found.CatalogBrand!.Brand);
        Assert.Equal("Sports car", found.CatalogType!.Type);
        Assert.Null(_service.FindCatalogItem(999));
    }

    [Fact]
    public void GetCatalogBrandsAndTypes_ReturnLookups()
    {
        Assert.Equal(2, _service.GetCatalogBrands().Count());
        Assert.Equal(2, _service.GetCatalogTypes().Count());
    }

    [Fact]
    public void CreateCatalogItem_GeneratesIdAndPersistsItem()
    {
        var item = new CatalogItem
        {
            Name = "Toranti Corsa",
            Price = 150000m,
            CatalogBrandId = 1,
            CatalogTypeId = 1
        };

        _service.CreateCatalogItem(item);

        Assert.True(item.Id > 0);
        Assert.Equal(4, _db.CatalogItems.Count());
        Assert.Equal(CatalogItem.DefaultPictureName, _service.FindCatalogItem(item.Id)!.PictureFileName);
    }

    [Fact]
    public void UpdateCatalogItem_PersistsChanges()
    {
        var item = _service.FindCatalogItem(2)!;
        item.Price = 95000m;

        _service.UpdateCatalogItem(item);

        Assert.Equal(95000m, _service.FindCatalogItem(2)!.Price);
    }

    [Fact]
    public void RemoveCatalogItem_DeletesItem()
    {
        var item = _service.FindCatalogItem(3)!;

        _service.RemoveCatalogItem(item);

        Assert.Null(_service.FindCatalogItem(3));
        Assert.Equal(2, _db.CatalogItems.Count());
    }

    [Fact]
    public void AvailableStock_IsMatchedByDateAndOverwrittenForSameDay()
    {
        var day = new DateTime(2026, 1, 15);

        _service.CreateAvailableStock(new CatalogItemsStock { CatalogItemId = 1, Date = day, AvailableStock = 5 });
        Assert.Equal(5, _service.GetAvailableStock(day.AddHours(9), 1));

        _service.CreateAvailableStock(new CatalogItemsStock { CatalogItemId = 1, Date = day, AvailableStock = 8 });
        Assert.Equal(8, _service.GetAvailableStock(day, 1));
        Assert.Single(_db.CatalogItemsStocks);

        Assert.Equal(0, _service.GetAvailableStock(day.AddDays(1), 1));
        Assert.Equal(0, _service.GetAvailableStock(day, 2));
    }

    [Fact]
    public void GetDiscount_ReturnsDiscountCoveringTheDay()
    {
        _db.DiscountItems.Add(new DiscountItem
        {
            Size = 0.1,
            Start = new DateTime(2026, 3, 1),
            End = new DateTime(2026, 3, 31)
        });
        _db.SaveChanges();

        Assert.Equal(0.1, _service.GetDiscount(new DateTime(2026, 3, 15, 13, 0, 0))!.Size);
        Assert.Equal(0.1, _service.GetDiscount(new DateTime(2026, 3, 31))!.Size);
        Assert.Null(_service.GetDiscount(new DateTime(2026, 4, 1)));
    }

    public void Dispose()
    {
        _service.Dispose();
        GC.SuppressFinalize(this);
    }
}
