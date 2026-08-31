using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eShopWCFService.Models.Infrastructure
{
    public static class PreconfiguredData
    {
        public static List<CatalogItem> GetPreconfiguredCatalogItems()
        {
            return new List<CatalogItem>()
            {
                new CatalogItem { Id =1, CatalogTypeId = 1, CatalogBrandId = 1, Description = "Ferrari SF90 Stradale plug-in hybrid supercar with 1000 cv", Name = "Ferrari SF90 Stradale", Price = 507000M, Picturefilename = "2.png" },
                new CatalogItem { Id =2, CatalogTypeId = 1, CatalogBrandId = 1, Description = "Ferrari 296 GTB with 663 cv V6 hybrid powertrain", Name = "Ferrari 296 GTB", Price = 320000M, Picturefilename = "11.png" },
                new CatalogItem { Id =3, CatalogTypeId = 2, CatalogBrandId = 1, Description = "Ferrari Roma front-engined V8 grand tourer", Name = "Ferrari Roma", Price = 222000M, Picturefilename = "7.png" },
                new CatalogItem { Id =4, CatalogTypeId = 3, CatalogBrandId = 1, Description = "Ferrari Purosangue four-door four-seater V12", Name = "Ferrari Purosangue", Price = 390000M, Picturefilename = "5.png" },
                new CatalogItem { Id =5, CatalogTypeId = 1, CatalogBrandId = 2, Description = "Lamborghini Revuelto V12 hybrid flagship", Name = "Lamborghini Revuelto", Price = 517000M, Picturefilename = "9.png" },
                new CatalogItem { Id =6, CatalogTypeId = 3, CatalogBrandId = 2, Description = "Lamborghini Urus Performante super SUV", Name = "Lamborghini Urus Performante", Price = 260000M, Picturefilename = "1.png" },
                new CatalogItem { Id =7, CatalogTypeId = 1, CatalogBrandId = 3, Description = "Porsche 911 GT3 RS track-focused flat-six", Name = "Porsche 911 GT3 RS", Price = 241000M, Picturefilename = "6.png" },
                new CatalogItem { Id =8, CatalogTypeId = 3, CatalogBrandId = 3, Description = "Porsche Cayenne Turbo GT performance SUV", Name = "Porsche Cayenne Turbo GT", Price = 198000M, Picturefilename = "3.png" },
                new CatalogItem { Id =9, CatalogTypeId = 1, CatalogBrandId = 4, Description = "Maserati MC20 with Nettuno twin-turbo V6", Name = "Maserati MC20", Price = 240000M, Picturefilename = "12.png" },
                new CatalogItem { Id =10, CatalogTypeId = 2, CatalogBrandId = 4, Description = "Maserati GranTurismo Trofeo V6 grand tourer", Name = "Maserati GranTurismo Trofeo", Price = 175000M, Picturefilename = "8.png" },
                new CatalogItem { Id =11, CatalogTypeId = 4, CatalogBrandId = 5, Description = "Carbon ceramic brake kit for track use", Name = "Carbon Ceramic Brake Kit", Price = 12500M, Picturefilename = "10.png" },
                new CatalogItem { Id =12, CatalogTypeId = 4, CatalogBrandId = 5, Description = "Forged alloy wheel set 20/21 inch staggered", Name = "Forged Alloy Wheel Set", Price = 8900M, Picturefilename = "4.png" },
            };
        }

        public static IEnumerable<CatalogBrand> GetPreconfiguredCatalogBrands()
        {
            return new List<CatalogBrand>()
            {
                new CatalogBrand() { Id =1, Brand = "Ferrari"},
                new CatalogBrand() { Id =2, Brand = "Lamborghini" },
                new CatalogBrand() { Id =3, Brand = "Porsche" },
                new CatalogBrand() { Id =4, Brand = "Maserati" },
                new CatalogBrand() { Id =5, Brand = "Other" }
            };
        }

        public static IEnumerable<DiscountItem> GetPreconfiguredDiscountItems()
        {
            return new List<DiscountItem>()
            {
                new DiscountItem() { Start = new DateTime(2017, 9, 18), End = new DateTime(2017, 9, 21), Size = 0.3f },
                new DiscountItem() { Start = new DateTime(2017, 9, 22), End = new DateTime(2017, 9, 26), Size = 0.25f },
                new DiscountItem() { Start = new DateTime(2017, 9, 27), End = new DateTime(2017, 9, 30), Size = 0.1f },
                new DiscountItem() { Start = new DateTime(2017, 10, 5), End = new DateTime(2017, 10, 20), Size = 0.5f },
                new DiscountItem() { Start = new DateTime(2017, 11, 13), End = new DateTime(2017, 11, 25), Size = 0.3f },
                new DiscountItem() { Start = new DateTime(2017, 12, 20), End = new DateTime(2017, 12, 25), Size = 0.25f },
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

        public static IEnumerable<CatalogItemsStock> GetPreconfiguredCatalogItemsStock()
        {
            return new List<CatalogItemsStock>()
            {
                new CatalogItemsStock() {StockId=1, CatalogItemId=1, Date=new DateTime(2017, 9, 20), AvailableStock=100},
                new CatalogItemsStock() {StockId=2, CatalogItemId=1, Date=new DateTime(2017, 9, 21), AvailableStock=120},
                new CatalogItemsStock() {StockId=3, CatalogItemId=1, Date=new DateTime(2017, 9, 22), AvailableStock=80},
                new CatalogItemsStock() {StockId=4, CatalogItemId=2, Date=new DateTime(2017, 9, 20), AvailableStock=45},
                new CatalogItemsStock() {StockId=5, CatalogItemId=4, Date=new DateTime(2017, 9, 25), AvailableStock=65},
                new CatalogItemsStock() {StockId=6, CatalogItemId=5, Date=new DateTime(2017, 9, 28), AvailableStock=22}
            };
        }
    }
}