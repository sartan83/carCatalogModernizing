using CarCatalog.Domain;

namespace CarCatalog.Application;

/// <summary>
/// The preconfigured catalog used both by the in-memory service and by database seeding.
/// Values match the legacy <c>PreconfiguredData</c> classes.
/// </summary>
public static class CatalogSeedData
{
    public static List<CatalogBrand> GetCatalogBrands() => new()
    {
        new CatalogBrand { Id = 1, Brand = "Velocari" },
        new CatalogBrand { Id = 2, Brand = "Toranti" },
        new CatalogBrand { Id = 3, Brand = "Nordwerk" },
        new CatalogBrand { Id = 4, Brand = "Aurelia" },
        new CatalogBrand { Id = 5, Brand = "Other" },
    };

    public static List<CatalogType> GetCatalogTypes() => new()
    {
        new CatalogType { Id = 1, Type = "Sports Car" },
        new CatalogType { Id = 2, Type = "GT" },
        new CatalogType { Id = 3, Type = "SUV" },
        new CatalogType { Id = 4, Type = "Spare Part" },
    };

    public static List<CatalogItem> GetCatalogItems() => new()
    {
        new CatalogItem { Id = 1, CatalogTypeId = 1, CatalogBrandId = 1, AvailableStock = 100, Description = "Velocari Strada SV plug-in hybrid supercar with 1000 cv", Name = "Velocari Strada SV", Price = 507000M, PictureFileName = "1.png" },
        new CatalogItem { Id = 2, CatalogTypeId = 1, CatalogBrandId = 1, AvailableStock = 100, Description = "Velocari Tipo 6 with 663 cv V6 hybrid powertrain", Name = "Velocari Tipo 6", Price = 320000M, PictureFileName = "2.png" },
        new CatalogItem { Id = 3, CatalogTypeId = 2, CatalogBrandId = 1, AvailableStock = 100, Description = "Velocari Aurora GT front-engined V8 grand tourer", Name = "Velocari Aurora GT", Price = 222000M, PictureFileName = "3.png" },
        new CatalogItem { Id = 4, CatalogTypeId = 3, CatalogBrandId = 1, AvailableStock = 100, Description = "Velocari Terra X four-door four-seater V12", Name = "Velocari Terra X", Price = 390000M, PictureFileName = "4.png" },
        new CatalogItem { Id = 5, CatalogTypeId = 1, CatalogBrandId = 2, AvailableStock = 100, Description = "Toranti Furente V12 hybrid flagship", Name = "Toranti Furente", Price = 517000M, PictureFileName = "5.png" },
        new CatalogItem { Id = 6, CatalogTypeId = 3, CatalogBrandId = 2, AvailableStock = 100, Description = "Toranti Monte S super SUV", Name = "Toranti Monte S", Price = 260000M, PictureFileName = "6.png" },
        new CatalogItem { Id = 7, CatalogTypeId = 1, CatalogBrandId = 3, AvailableStock = 100, Description = "Nordwerk RS9 track-focused flat-six", Name = "Nordwerk RS9", Price = 241000M, PictureFileName = "7.png" },
        new CatalogItem { Id = 8, CatalogTypeId = 3, CatalogBrandId = 3, AvailableStock = 100, Description = "Nordwerk Terra Turbo performance SUV", Name = "Nordwerk Terra Turbo", Price = 198000M, PictureFileName = "8.png" },
        new CatalogItem { Id = 9, CatalogTypeId = 1, CatalogBrandId = 4, AvailableStock = 100, Description = "Aurelia Nettare S with twin-turbo V6 and carbon tub", Name = "Aurelia Nettare S", Price = 240000M, PictureFileName = "9.png" },
        new CatalogItem { Id = 10, CatalogTypeId = 2, CatalogBrandId = 4, AvailableStock = 100, Description = "Aurelia Granluce V6 grand tourer", Name = "Aurelia Granluce", Price = 175000M, PictureFileName = "10.png" },
        new CatalogItem { Id = 11, CatalogTypeId = 4, CatalogBrandId = 5, AvailableStock = 100, Description = "Carbon ceramic brake kit for track use", Name = "Carbon Ceramic Brake Kit", Price = 12500M, PictureFileName = "11.png" },
        new CatalogItem { Id = 12, CatalogTypeId = 4, CatalogBrandId = 5, AvailableStock = 100, Description = "Forged alloy wheel set 20/21 inch staggered", Name = "Forged Alloy Wheel Set", Price = 8900M, PictureFileName = "12.png" },
    };

    public static List<CatalogItemsStock> GetCatalogItemsStock() => new()
    {
        new CatalogItemsStock { StockId = 1, CatalogItemId = 1, Date = new DateTime(2017, 9, 20), AvailableStock = 100 },
        new CatalogItemsStock { StockId = 2, CatalogItemId = 1, Date = new DateTime(2017, 9, 21), AvailableStock = 120 },
        new CatalogItemsStock { StockId = 3, CatalogItemId = 1, Date = new DateTime(2017, 9, 22), AvailableStock = 80 },
        new CatalogItemsStock { StockId = 4, CatalogItemId = 2, Date = new DateTime(2017, 9, 20), AvailableStock = 45 },
        new CatalogItemsStock { StockId = 5, CatalogItemId = 4, Date = new DateTime(2017, 9, 25), AvailableStock = 65 },
        new CatalogItemsStock { StockId = 6, CatalogItemId = 5, Date = new DateTime(2017, 9, 28), AvailableStock = 22 },
    };

    public static List<DiscountItem> GetDiscountItems() => new()
    {
        new DiscountItem { Id = 1, Start = new DateTime(2017, 9, 18), End = new DateTime(2017, 9, 21), Size = 0.3f },
        new DiscountItem { Id = 2, Start = new DateTime(2017, 9, 22), End = new DateTime(2017, 9, 26), Size = 0.25f },
        new DiscountItem { Id = 3, Start = new DateTime(2017, 9, 27), End = new DateTime(2017, 9, 30), Size = 0.1f },
        new DiscountItem { Id = 4, Start = new DateTime(2017, 10, 5), End = new DateTime(2017, 10, 20), Size = 0.5f },
        new DiscountItem { Id = 5, Start = new DateTime(2017, 11, 13), End = new DateTime(2017, 11, 25), Size = 0.3f },
        new DiscountItem { Id = 6, Start = new DateTime(2017, 12, 20), End = new DateTime(2017, 12, 25), Size = 0.25f },
    };
}
