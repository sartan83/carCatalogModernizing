using CarCatalog.Application;
using CarCatalog.Domain;
using Microsoft.EntityFrameworkCore;

namespace CarCatalog.Infrastructure;

public class EfCatalogService : ICatalogService
{
    private readonly CatalogDbContext db;

    public EfCatalogService(CatalogDbContext db)
    {
        this.db = db;
    }

    public CatalogItem? FindCatalogItem(int id)
    {
        return db.CatalogItems
            .Include(c => c.CatalogBrand)
            .Include(c => c.CatalogType)
            .FirstOrDefault(ci => ci.Id == id);
    }

    public IReadOnlyList<CatalogBrand> GetCatalogBrands() => db.CatalogBrands.OrderBy(b => b.Id).ToList();

    public IReadOnlyList<CatalogType> GetCatalogTypes() => db.CatalogTypes.OrderBy(t => t.Id).ToList();

    public IReadOnlyList<CatalogItem> GetCatalogItems(int brandIdFilter, int typeIdFilter)
    {
        return db.CatalogItems
            .Include(c => c.CatalogBrand)
            .Include(c => c.CatalogType)
            .Where(c => (brandIdFilter == 0 || c.CatalogBrandId == brandIdFilter)
                && (typeIdFilter == 0 || c.CatalogTypeId == typeIdFilter))
            .OrderBy(c => c.Id)
            .ToList();
    }

    public PaginatedItems<CatalogItem> GetCatalogItemsPaginated(int pageSize, int pageIndex)
    {
        var totalItems = db.CatalogItems.LongCount();

        var itemsOnPage = db.CatalogItems
            .Include(c => c.CatalogBrand)
            .Include(c => c.CatalogType)
            .OrderBy(c => c.Id)
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToList();

        return new PaginatedItems<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage);
    }

    public void CreateCatalogItem(CatalogItem catalogItem)
    {
        db.CatalogItems.Add(catalogItem);
        db.SaveChanges();
    }

    public void UpdateCatalogItem(CatalogItem catalogItem)
    {
        var tracked = db.CatalogItems.Find(catalogItem.Id);
        if (tracked == null)
        {
            return;
        }

        db.Entry(tracked).CurrentValues.SetValues(catalogItem);
        db.SaveChanges();
    }

    public void RemoveCatalogItem(CatalogItem catalogItem)
    {
        var tracked = db.CatalogItems.Find(catalogItem.Id);
        if (tracked == null)
        {
            return;
        }

        db.CatalogItems.Remove(tracked);
        db.SaveChanges();
    }

    public int GetAvailableStock(DateTime date, int catalogItemId)
    {
        var day = date.Date;

        return db.CatalogItemsStocks
            .Where(s => s.CatalogItemId == catalogItemId && s.Date == day)
            .Select(s => s.AvailableStock)
            .FirstOrDefault();
    }

    public void CreateAvailableStock(CatalogItemsStock catalogItemsStock)
    {
        var day = catalogItemsStock.Date.Date;

        var existing = db.CatalogItemsStocks
            .FirstOrDefault(s => s.CatalogItemId == catalogItemsStock.CatalogItemId && s.Date == day);

        if (existing != null)
        {
            existing.AvailableStock = catalogItemsStock.AvailableStock;
        }
        else
        {
            catalogItemsStock.StockId = (db.CatalogItemsStocks.Max(s => (int?)s.StockId) ?? 0) + 1;
            catalogItemsStock.Date = day;
            db.CatalogItemsStocks.Add(catalogItemsStock);
        }

        db.SaveChanges();
    }

    public DiscountItem? GetDiscount(DateTime day)
    {
        var date = day.Date;

        return db.DiscountItems
            .Where(d => d.Start <= date && d.End >= date)
            .OrderBy(d => d.Id)
            .FirstOrDefault();
    }
}
