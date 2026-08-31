using System.ServiceModel;
using CarCatalog.Domain;
using CarCatalog.ServiceContracts;

namespace CarCatalogWinForms;

/// <summary>
/// Replaces the svcutil-generated <c>Connected Services</c> proxy: the contract assembly is shared with the
/// service, so the client is a plain <see cref="ClientBase{TChannel}"/> over the same interface.
/// </summary>
public class CatalogServiceClient : ClientBase<ICatalogWcfService>, ICatalogWcfService
{
    public CatalogServiceClient(string address)
        : base(new BasicHttpBinding(), new EndpointAddress(address))
    {
    }

    public CatalogItem FindCatalogItem(int id) => Channel.FindCatalogItem(id);

    public List<CatalogBrand> GetCatalogBrands() => Channel.GetCatalogBrands();

    public List<CatalogType> GetCatalogTypes() => Channel.GetCatalogTypes();

    public List<CatalogItem> GetCatalogItems(int brandIdFilter, int typeIdFilter) =>
        Channel.GetCatalogItems(brandIdFilter, typeIdFilter);

    public int GetAvailableStock(DateTime date, int catalogItemId) => Channel.GetAvailableStock(date, catalogItemId);

    public void CreateAvailableStock(CatalogItemsStock catalogItemsStock) => Channel.CreateAvailableStock(catalogItemsStock);

    public void CreateCatalogItem(CatalogItem catalogItem) => Channel.CreateCatalogItem(catalogItem);

    public void UpdateCatalogItem(CatalogItem catalogItem) => Channel.UpdateCatalogItem(catalogItem);

    public void RemoveCatalogItem(CatalogItem catalogItem) => Channel.RemoveCatalogItem(catalogItem);

    public DiscountItem GetDiscount(DateTime day) => Channel.GetDiscount(day);
}
