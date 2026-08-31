using CarCatalog.Application;
using CarCatalog.Domain;
using CarCatalog.ServiceContracts;

namespace CarCatalog.WcfService;

/// <summary>
/// SOAP facade over the shared catalog service; the legacy service talked to EF6 directly.
/// </summary>
public class CatalogWcfService(ICatalogService service) : ICatalogWcfService
{
    public CatalogItem? FindCatalogItem(int id) => service.FindCatalogItem(id);

    public List<CatalogBrand> GetCatalogBrands() => [.. service.GetCatalogBrands()];

    public List<CatalogType> GetCatalogTypes() => [.. service.GetCatalogTypes()];

    public List<CatalogItem> GetCatalogItems(int brandIdFilter, int typeIdFilter) =>
        [.. service.GetCatalogItems(brandIdFilter, typeIdFilter)];

    public int GetAvailableStock(DateTime date, int catalogItemId) => service.GetAvailableStock(date, catalogItemId);

    public void CreateAvailableStock(CatalogItemsStock catalogItemsStock) => service.CreateAvailableStock(catalogItemsStock);

    public void CreateCatalogItem(CatalogItem catalogItem) => service.CreateCatalogItem(catalogItem);

    public void UpdateCatalogItem(CatalogItem catalogItem) => service.UpdateCatalogItem(catalogItem);

    public void RemoveCatalogItem(CatalogItem catalogItem) => service.RemoveCatalogItem(catalogItem);

    public DiscountItem? GetDiscount(DateTime day) => service.GetDiscount(day);
}
