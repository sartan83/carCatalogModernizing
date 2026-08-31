using CarCatalog.Domain;

namespace CarCatalog.Application;

/// <summary>
/// Catalog operations shared by every front end. It is the union of the two legacy
/// <c>ICatalogService</c> contracts: the web apps' CRUD and pagination plus the N-Tier
/// service's stock and discount operations.
/// </summary>
public interface ICatalogService
{
    CatalogItem? FindCatalogItem(int id);

    IReadOnlyList<CatalogBrand> GetCatalogBrands();

    IReadOnlyList<CatalogType> GetCatalogTypes();

    IReadOnlyList<CatalogItem> GetCatalogItems(int brandIdFilter, int typeIdFilter);

    PaginatedItems<CatalogItem> GetCatalogItemsPaginated(int pageSize, int pageIndex);

    void CreateCatalogItem(CatalogItem catalogItem);

    void UpdateCatalogItem(CatalogItem catalogItem);

    void RemoveCatalogItem(CatalogItem catalogItem);

    int GetAvailableStock(DateTime date, int catalogItemId);

    void CreateAvailableStock(CatalogItemsStock catalogItemsStock);

    DiscountItem? GetDiscount(DateTime day);
}
