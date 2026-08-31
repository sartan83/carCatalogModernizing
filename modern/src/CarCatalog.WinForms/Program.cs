using System;
using System.IO;
using System.Windows.Forms;
using CarCatalog.ServiceContracts;
using CarCatalogWinForms.Controllers;
using Microsoft.Extensions.Configuration;

namespace CarCatalogWinForms
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables("CARCATALOG_")
                .Build();

            var address = configuration["CatalogService:Address"]
                ?? "http://localhost:62314" + CarCatalog.ServiceContracts.ServiceAddresses.CatalogService;

            CatalogView catalogView = new CatalogView();
            ICatalogWcfService service = new CatalogServiceClient(address);
            CatalogController catalogController = new CatalogController(service, catalogView);

            catalogController.LoadView();
            catalogView.ShowDialog();
        }
    }
}
