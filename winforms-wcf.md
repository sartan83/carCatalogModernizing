# eShopModernizingWCFWinForms
> ### DISCLAIMER
> **IMPORTANT:** The current state of this sample application is **BETA**, consider it version a 0.01, therefore, many areas will be improved and changed significantly. This is purely built with the purpose of showing off a concept/demo at this point. 

![WinForms](assets/winForms.PNG)

## Introduction
This sample project shows how to modernize a traditional Line of Business WinForms applications. The WinForms front-end is a fictional inventory app which allows it's users to track inventory for the cars and spare parts being sold through their dealership. You can look at inventory counts for particular items at different calendar dates. 

It is the starting point for modernization work such as:
- Containerizing the WCF Service and consuming it from the WinForms App
- Containerizing the SQL database back-end
- Publishing the SQL database to Azure
- Calling an ASP.NET Core Web API from a WinForms App
- Updating the WinForms App to High DPI compatibility
- Deploying the WinForms App via Centennial

## Repository and Project Structure

The solution lives in `eShopLegacyNTier` and is the raw, pre-modernization version: WinForms (no High DPI support) interfacing with a local WCF service which talks to a local SQL database.

Each component of the solution is broken into its own project (frontend, WCF, etc).

## Application Walkthrough

We've broken out a full walkthrough of converting from a legacy version to the modernized version in the wiki. Go [check out the wiki](https://github.com/dotnet-architecture/eShopModernizingWCFWinForms/wiki) to get a step-by-step explanation of the features called out in this readme.
