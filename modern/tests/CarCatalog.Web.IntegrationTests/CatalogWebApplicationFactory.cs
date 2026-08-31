using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace CarCatalog.Web.IntegrationTests;

/// <summary>
/// Hosts the app with the in-memory catalog so the tests need no database.
/// </summary>
public class CatalogWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config => config.AddInMemoryCollection(
            new Dictionary<string, string?> { ["Catalog:UseMockData"] = "true" }));

        return base.CreateHost(builder);
    }
}
