using Catalog.Application.Models;
using Catalog.Domain.Entities;

namespace Catalog.Application.Abstractions;

/// <summary>
/// Canonical catalog contract: the union of the MVC/WebForms service interface and
/// the WCF service contract.
/// </summary>
public interface ICatalogService : IDisposable
{
    CatalogItem? FindCatalogItem(int id);

    IEnumerable<CatalogBrand> GetCatalogBrands();

    IEnumerable<CatalogType> GetCatalogTypes();

    PaginatedItems<CatalogItem> GetCatalogItemsPaginated(int pageSize, int pageIndex);

    IReadOnlyList<CatalogItem> GetCatalogItems(int brandIdFilter, int typeIdFilter);

    void CreateCatalogItem(CatalogItem catalogItem);

    void UpdateCatalogItem(CatalogItem catalogItem);

    void RemoveCatalogItem(CatalogItem catalogItem);

    int GetAvailableStock(DateTime date, int catalogItemId);

    void CreateAvailableStock(CatalogItemsStock catalogItemsStock);

    DiscountItem? GetDiscount(DateTime day);
}
