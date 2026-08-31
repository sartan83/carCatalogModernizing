using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarCatalogLegacyWebForms.Models.Infrastructure
{
    public static class PreconfiguredData
    {
        public static List<CatalogItem> GetPreconfiguredCatalogItems()
        {
            return new List<CatalogItem>()
            {
                new CatalogItem { Id =1, CatalogTypeId = 1, CatalogBrandId = 1, AvailableStock = 100, Description = "Velocari Strada SV plug-in hybrid supercar with 1000 cv", Name = "Velocari Strada SV", Price = 507000M, PictureFileName = "1.png" },
                new CatalogItem { Id =2, CatalogTypeId = 1, CatalogBrandId = 1, AvailableStock = 100, Description = "Velocari Tipo 6 with 663 cv V6 hybrid powertrain", Name = "Velocari Tipo 6", Price = 320000M, PictureFileName = "2.png" },
                new CatalogItem { Id =3, CatalogTypeId = 2, CatalogBrandId = 1, AvailableStock = 100, Description = "Velocari Aurora GT front-engined V8 grand tourer", Name = "Velocari Aurora GT", Price = 222000M, PictureFileName = "3.png" },
                new CatalogItem { Id =4, CatalogTypeId = 3, CatalogBrandId = 1, AvailableStock = 100, Description = "Velocari Terra X four-door four-seater V12", Name = "Velocari Terra X", Price = 390000M, PictureFileName = "4.png" },
                new CatalogItem { Id =5, CatalogTypeId = 1, CatalogBrandId = 2, AvailableStock = 100, Description = "Toranti Furente V12 hybrid flagship", Name = "Toranti Furente", Price = 517000M, PictureFileName = "5.png" },
                new CatalogItem { Id =6, CatalogTypeId = 3, CatalogBrandId = 2, AvailableStock = 100, Description = "Toranti Monte S super SUV", Name = "Toranti Monte S", Price = 260000M, PictureFileName = "6.png" },
                new CatalogItem { Id =7, CatalogTypeId = 1, CatalogBrandId = 3, AvailableStock = 100, Description = "Nordwerk RS9 track-focused flat-six", Name = "Nordwerk RS9", Price = 241000M, PictureFileName = "7.png" },
                new CatalogItem { Id =8, CatalogTypeId = 3, CatalogBrandId = 3, AvailableStock = 100, Description = "Nordwerk Terra Turbo performance SUV", Name = "Nordwerk Terra Turbo", Price = 198000M, PictureFileName = "8.png" },
                new CatalogItem { Id =9, CatalogTypeId = 1, CatalogBrandId = 4, AvailableStock = 100, Description = "Aurelia Nettare S with twin-turbo V6 and carbon tub", Name = "Aurelia Nettare S", Price = 240000M, PictureFileName = "9.png" },
                new CatalogItem { Id =10, CatalogTypeId = 2, CatalogBrandId = 4, AvailableStock = 100, Description = "Aurelia Granluce V6 grand tourer", Name = "Aurelia Granluce", Price = 175000M, PictureFileName = "10.png" },
                new CatalogItem { Id =11, CatalogTypeId = 4, CatalogBrandId = 5, AvailableStock = 100, Description = "Carbon ceramic brake kit for track use", Name = "Carbon Ceramic Brake Kit", Price = 12500M, PictureFileName = "11.png" },
                new CatalogItem { Id =12, CatalogTypeId = 4, CatalogBrandId = 5, AvailableStock = 100, Description = "Forged alloy wheel set 20/21 inch staggered", Name = "Forged Alloy Wheel Set", Price = 8900M, PictureFileName = "12.png" },
            };
        }

        public static IEnumerable<CatalogBrand> GetPreconfiguredCatalogBrands()
        {
            return new List<CatalogBrand>()
            {
                new CatalogBrand() { Id =1, Brand = "Velocari"},
                new CatalogBrand() { Id =2, Brand = "Toranti" },
                new CatalogBrand() { Id =3, Brand = "Nordwerk" },
                new CatalogBrand() { Id =4, Brand = "Aurelia" },
                new CatalogBrand() { Id =5, Brand = "Other" }
            };
        }

        public static IEnumerable<CatalogType> GetPreconfiguredCatalogTypes()
        {
            return new List<CatalogType>()
            {
                new CatalogType() { Id =1, Type = "Sports Car"},
                new CatalogType() { Id =2, Type = "GT" },
                new CatalogType() { Id =3, Type = "SUV" },
                new CatalogType() { Id =4, Type = "Spare Part" }
            };
        }
    }
}