using CarCatalog.Domain;

namespace CarCatalog.Application;

/// <summary>
/// Replacement for the legacy <c>CatalogServiceMock</c> classes: the catalog lives in memory so
/// the apps can run without a database. Unlike the legacy mocks, brand and type are always
/// populated on the items it returns and <see cref="GetDiscount"/> is implemented.
/// </summary>
public class InMemoryCatalogService : ICatalogService
{
    private readonly object gate = new();
    private readonly List<CatalogItem> catalogItems;
    private readonly List<CatalogBrand> catalogBrands;
    private readonly List<CatalogType> catalogTypes;
    private readonly List<CatalogItemsStock> catalogItemsStock;
    private readonly List<DiscountItem> discountItems;

    public InMemoryCatalogService()
    {
        catalogBrands = CatalogSeedData.GetCatalogBrands();
        catalogTypes = CatalogSeedData.GetCatalogTypes();
        catalogItemsStock = CatalogSeedData.GetCatalogItemsStock();
        discountItems = CatalogSeedData.GetDiscountItems();
        catalogItems = CatalogSeedData.GetCatalogItems();

        foreach (var item in catalogItems)
        {
            Compose(item);
        }
    }

    public CatalogItem? FindCatalogItem(int id)
    {
        lock (gate)
        {
            return catalogItems.FirstOrDefault(x => x.Id == id);
        }
    }

    public IReadOnlyList<CatalogBrand> GetCatalogBrands() => catalogBrands;

    public IReadOnlyList<CatalogType> GetCatalogTypes() => catalogTypes;

    public IReadOnlyList<CatalogItem> GetCatalogItems(int brandIdFilter, int typeIdFilter)
    {
        lock (gate)
        {
            return catalogItems
                .Where(x => (brandIdFilter == 0 || x.CatalogBrandId == brandIdFilter)
                    && (typeIdFilter == 0 || x.CatalogTypeId == typeIdFilter))
                .ToList();
        }
    }

    public PaginatedItems<CatalogItem> GetCatalogItemsPaginated(int pageSize, int pageIndex)
    {
        lock (gate)
        {
            var itemsOnPage = catalogItems
                .OrderBy(c => c.Id)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToList();

            return new PaginatedItems<CatalogItem>(pageIndex, pageSize, catalogItems.Count, itemsOnPage);
        }
    }

    public void CreateCatalogItem(CatalogItem catalogItem)
    {
        lock (gate)
        {
            catalogItem.Id = catalogItems.Count == 0 ? 1 : catalogItems.Max(i => i.Id) + 1;
            Compose(catalogItem);
            catalogItems.Add(catalogItem);
        }
    }

    public void UpdateCatalogItem(CatalogItem catalogItem)
    {
        lock (gate)
        {
            var index = catalogItems.FindIndex(i => i.Id == catalogItem.Id);
            if (index < 0)
            {
                return;
            }

            Compose(catalogItem);
            catalogItems[index] = catalogItem;
        }
    }

    public void RemoveCatalogItem(CatalogItem catalogItem)
    {
        lock (gate)
        {
            catalogItems.RemoveAll(i => i.Id == catalogItem.Id);
        }
    }

    public int GetAvailableStock(DateTime date, int catalogItemId)
    {
        lock (gate)
        {
            return catalogItemsStock
                .FirstOrDefault(s => s.CatalogItemId == catalogItemId && s.Date.Date == date.Date)?.AvailableStock ?? 0;
        }
    }

    public void CreateAvailableStock(CatalogItemsStock catalogItemsStock)
    {
        lock (gate)
        {
            var existing = this.catalogItemsStock.FirstOrDefault(s =>
                s.CatalogItemId == catalogItemsStock.CatalogItemId && s.Date.Date == catalogItemsStock.Date.Date);

            if (existing != null)
            {
                existing.AvailableStock = catalogItemsStock.AvailableStock;
                return;
            }

            catalogItemsStock.StockId = this.catalogItemsStock.Count == 0
                ? 1
                : this.catalogItemsStock.Max(s => s.StockId) + 1;
            this.catalogItemsStock.Add(catalogItemsStock);
        }
    }

    public DiscountItem? GetDiscount(DateTime day)
    {
        lock (gate)
        {
            return discountItems.FirstOrDefault(d => d.Start.Date <= day.Date && d.End.Date >= day.Date);
        }
    }

    private void Compose(CatalogItem item)
    {
        item.CatalogBrand = catalogBrands.FirstOrDefault(b => b.Id == item.CatalogBrandId);
        item.CatalogType = catalogTypes.FirstOrDefault(t => t.Id == item.CatalogTypeId);
    }
}
