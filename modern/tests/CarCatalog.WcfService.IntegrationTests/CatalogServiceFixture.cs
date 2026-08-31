using System.ServiceModel;
using CarCatalog.ServiceContracts;
using CarCatalog.WcfService;
using Microsoft.AspNetCore.Builder;

namespace CarCatalog.WcfService.IntegrationTests;

/// <summary>
/// Runs the real CoreWCF host on an ephemeral port over the in-memory catalog, so the tests exercise
/// the actual SOAP stack rather than the service class directly.
/// </summary>
public sealed class CatalogServiceFixture : IAsyncLifetime
{
    private WebApplication? app;

    public string Address { get; private set; } = string.Empty;

    public async Task InitializeAsync()
    {
        app = CatalogServiceHost.Build(["--Catalog:UseMockData=true", "--urls=http://127.0.0.1:0"]);
        await app.StartAsync();

        Address = app.Urls.First() + ServiceAddresses.CatalogService;
    }

    public ICatalogWcfService CreateChannel() =>
        new ChannelFactory<ICatalogWcfService>(new BasicHttpBinding(), new EndpointAddress(Address)).CreateChannel();

    public async Task DisposeAsync()
    {
        if (app != null)
        {
            await app.DisposeAsync();
        }
    }
}
