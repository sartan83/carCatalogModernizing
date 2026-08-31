
# carCatalogModernizing - Automotive catalog apps on .NET Framework (ASP.NET MVC, WebForms, WCF + WinForms)

This repo contains three sample back-office automotive catalog applications built on .NET Framework:

- **ASP.NET MVC 5** web app — `eShopLegacyMVCSolution/eShopLegacyMVC.sln`
- **ASP.NET WebForms** web app — `eShopLegacyWebFormsSolution/eShopLegacyWebForms.sln`
- **N-Tier app** — a WCF service with a WinForms desktop client — `eShopLegacyNTier/eShopLegacyNTier.sln`

## What the apps do

Each app is the internal back-office of a car maker/dealer group (an "Auto Catalog Manager") so employees can update the vehicle catalog: sports cars, GTs, SUVs and spare parts from fictional marques such as Velocari, Toranti, Nordwerk and Aurelia. They are CRUD applications over a SQL Server database.

The catalog domain is modelled with three entities: `CatalogBrand` (the marque), `CatalogType` (the vehicle/part category) and `CatalogItem` (the vehicle or part itself, with price, stock and picture).

The MVC and WebForms apps are nearly identical in UI and business features; both exist so the same application can be compared across the two technologies.

![image](https://user-images.githubusercontent.com/1712635/30354210-0638f3b2-97e0-11e7-82c5-df18197ccdbd.png)

### WinForms + WCF application

The WinForms client is a catalog/inventory app that reads and writes through a WCF service. Read more about it [here](./winforms-wcf.md).

## Running the apps

Open a solution in Visual Studio on Windows (or build it with `msbuild`), restore the NuGet packages and run it with IIS Express. For the N-Tier sample, start the WCF service before the WinForms client.

Autofac wires up the dependencies in each app (`Modules/ApplicationModule.cs`), and `CatalogDBContext` (Entity Framework) provides persistence, with Hi-Lo sequences generating the catalog ids.

### Mock-data or a real SQL Server database

Every app can either connect to SQL Server or serve an in-memory catalog when no database is available — useful for testing and demos. The choice is per app in its `Web.config`/`App.config`:

- `UseMockData` — `true` serves the in-memory catalog, `false` uses SQL Server.
- `UseCustomizationData` — `true` seeds the catalog from the CSV files and pictures zip in the app's `Setup` folder instead of `Models/Infrastructure/PreconfiguredData.cs`, so brands, types and items can be changed without recompiling.
