
# carCatalogModernizing - Legacy ASP.NET (MVC and WebForms) and N-Tier (WCF + WinForms) automotive catalog apps

This repo provides three sample hypothetical legacy automotive catalog apps built on .NET Framework, in the state they are in **before** any modernization work:

- **ASP.NET MVC** web app (`eShopLegacyMVCSolution`, plus an SDK-style ported variant)
- **ASP.NET WebForms** web app (`eShopLegacyWebFormsSolution`)
- **N-Tier app** made of a WCF service and a WinForms desktop client (`eShopLegacyNTier`)

They are the starting point for a Lift and Shift modernization (Windows Containers, Azure Container Instances, Windows Server VMs, AKS, Azure Web App for Containers); none of that has been applied here yet, so the repo contains no Dockerfiles, compose files or cloud manifests.

## Related Guide/eBook

The modernization guidance these samples were built for is available as a free guide/eBook (2nd Edition), and the PDF/EPUB/MOBI copies are in the [`Docs`](./Docs) folder:

<img src="https://github.com/dotnet/docs/raw/master/docs/architecture/modernize-with-azure-containers/media/index/web-application-guide-cover-image.png" width="300">

.PDF download: https://aka.ms/liftandshiftwithcontainersebook

Modernizing with Windows Containers significantly improves the deployments for DevOps, without having to change the app's architecture or C# code.

## What the apps do

The sample apps are simple back-office apps for a car maker/dealer group (an "Auto Catalog Manager") so employees can update the vehicle catalog: sports cars, GTs, SUVs and spare parts from fictional marques such as Velocari, Toranti, Nordwerk and Aurelia.
They are therefore simple CRUD applications updating data in a SQL Server database.

The catalog domain is modelled with three entities: `CatalogBrand` (the marque), `CatalogType` (the vehicle/part category) and `CatalogItem` (the vehicle or part itself, with price, stock and picture).

### UI and business features

The WebForms and MVC apps are pretty similar in regards to UI and business features. Both versions exist so you can compare, depending on what technology your existing apps use (ASP.NET MVC or Web Forms).

![image](https://user-images.githubusercontent.com/1712635/30354210-0638f3b2-97e0-11e7-82c5-df18197ccdbd.png)

### WinForms + WCF application

The WinForms application is a catalog/inventory manager that uses a WCF service as its back-end. Read more about the WinForms + WCF sample [here](./winforms-wcf.md).

## Running the apps

Each solution is a .NET Framework solution and builds with Visual Studio (or `msbuild`) on Windows:

| App | Solution |
| --- | --- |
| MVC | `eShopLegacyMVCSolution/eShopLegacyMVC.sln` |
| WebForms | `eShopLegacyWebFormsSolution/eShopLegacyWebForms.sln` |
| WCF + WinForms | `eShopLegacyNTier/eShopLegacyNTier.sln` |

Open a solution, restore NuGet packages, and run it with IIS Express. For the N-Tier sample, start the WCF service before the WinForms client.

### Choose in-memory mock-data or a real SQL Server database

The apps can either connect to a real database to get/update the vehicle catalog, or use mock-data when the database is not available and you just need to test/demo the app. The option is configured per application in its `Web.config`/`App.config` (`UseMockData`).

Catalog data comes from `Models/Infrastructure/PreconfiguredData.cs`. Setting `UseCustomizationData` instead loads the catalog from the CSV files (and pictures zip) under each app's `Setup` folder, so brands, types and items can be changed without recompiling.

### Azure preparation scripts

The PowerShell scripts under [`Setup`](./Setup) provision the Azure resources (App Service plan, Web App with a managed identity, Azure SQL server and database) used as the target of a later migration.
