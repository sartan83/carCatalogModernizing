using System.ServiceModel;
using CarCatalog.Domain;

namespace CarCatalog.ServiceContracts;

/// <summary>
/// The SOAP contract the WinForms client talks to. The contract name and namespace match the legacy
/// <c>CarCatalogWCFService.ICatalogService</c> so existing clients keep working; the C# name differs only
/// to avoid clashing with the in-process <c>CarCatalog.Application.ICatalogService</c>.
/// </summary>
[ServiceContract(Name = "ICatalogService", Namespace = "http://tempuri.org/")]
public interface ICatalogWcfService
{
    [OperationContract]
    CatalogItem? FindCatalogItem(int id);

    [OperationContract]
    List<CatalogBrand> GetCatalogBrands();

    [OperationContract]
    List<CatalogItem> GetCatalogItems(int brandIdFilter, int typeIdFilter);

    [OperationContract]
    List<CatalogType> GetCatalogTypes();

    [OperationContract]
    int GetAvailableStock(DateTime date, int catalogItemId);

    [OperationContract]
    void CreateAvailableStock(CatalogItemsStock catalogItemsStock);

    [OperationContract]
    void CreateCatalogItem(CatalogItem catalogItem);

    [OperationContract]
    void UpdateCatalogItem(CatalogItem catalogItem);

    [OperationContract]
    void RemoveCatalogItem(CatalogItem catalogItem);

    [OperationContract]
    DiscountItem? GetDiscount(DateTime day);
}
