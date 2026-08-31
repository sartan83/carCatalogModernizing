using Catalog.Application.Abstractions;
using Catalog.Application.Models;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Services;

public class CatalogService : ICatalogService
{
    private readonly CatalogDbContext _db;

    public CatalogService(CatalogDbContext db)
    {
        _db = db;
    }

    public PaginatedItems<CatalogItem> GetCatalogItemsPaginated(int pageSize, int pageIndex)
    {
        var totalItems = _db.CatalogItems.LongCount();

        var itemsOnPage = _db.CatalogItems
            .Include(c => c.CatalogBrand)
            .Include(c => c.CatalogType)
            .OrderBy(c => c.Id)
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToList();

        return new PaginatedItems<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage);
    }

    public IReadOnlyList<CatalogItem> GetCatalogItems(int brandIdFilter, int typeIdFilter)
    {
        var query = _db.CatalogItems
            .Include(c => c.CatalogBrand)
            .Include(c => c.CatalogType)
            .AsQueryable();

        if (brandIdFilter != 0)
        {
            query = query.Where(c => c.CatalogBrandId == brandIdFilter);
        }

        if (typeIdFilter != 0)
        {
            query = query.Where(c => c.CatalogTypeId == typeIdFilter);
        }

        return query.OrderBy(c => c.Id).ToList();
    }

    public CatalogItem? FindCatalogItem(int id)
    {
        return _db.CatalogItems
            .Include(c => c.CatalogBrand)
            .Include(c => c.CatalogType)
            .FirstOrDefault(ci => ci.Id == id);
    }

    public IEnumerable<CatalogType> GetCatalogTypes() => _db.CatalogTypes.ToList();

    public IEnumerable<CatalogBrand> GetCatalogBrands() => _db.CatalogBrands.ToList();

    public void CreateCatalogItem(CatalogItem catalogItem)
    {
        _db.CatalogItems.Add(catalogItem);
        _db.SaveChanges();
    }

    public void UpdateCatalogItem(CatalogItem catalogItem)
    {
        _db.Entry(catalogItem).State = EntityState.Modified;
        _db.SaveChanges();
    }

    public void RemoveCatalogItem(CatalogItem catalogItem)
    {
        _db.CatalogItems.Remove(catalogItem);
        _db.SaveChanges();
    }

    public int GetAvailableStock(DateTime date, int catalogItemId)
    {
        var stock = _db.CatalogItemsStocks
            .Where(s => s.CatalogItemId == catalogItemId)
            .AsEnumerable()
            .FirstOrDefault(s => s.Date.Date == date.Date);

        return stock?.AvailableStock ?? 0;
    }

    public void CreateAvailableStock(CatalogItemsStock catalogItemsStock)
    {
        var existing = _db.CatalogItemsStocks
            .Where(s => s.CatalogItemId == catalogItemsStock.CatalogItemId)
            .AsEnumerable()
            .FirstOrDefault(s => s.Date.Date == catalogItemsStock.Date.Date);

        if (existing != null)
        {
            existing.AvailableStock = catalogItemsStock.AvailableStock;
            _db.Entry(existing).State = EntityState.Modified;
        }
        else
        {
            _db.CatalogItemsStocks.Add(catalogItemsStock);
        }

        _db.SaveChanges();
    }

    public DiscountItem? GetDiscount(DateTime day)
    {
        return _db.DiscountItems
            .AsEnumerable()
            .FirstOrDefault(d => d.Start.Date <= day.Date && d.End.Date >= day.Date);
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }
}
