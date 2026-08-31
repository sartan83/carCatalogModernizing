namespace Catalog.Domain.Entities;

/// <summary>Stock level of a catalog item on a given day.</summary>
public class CatalogItemsStock
{
    public int StockId { get; set; }

    public DateTime Date { get; set; }

    public int CatalogItemId { get; set; }

    public int AvailableStock { get; set; }
}
