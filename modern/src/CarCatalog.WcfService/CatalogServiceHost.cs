using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using CarCatalog.Infrastructure;
using CarCatalog.ServiceContracts;

namespace CarCatalog.WcfService;

/// <summary>
/// Builds the CoreWCF host. Shared with the tests, which run it on an ephemeral port.
/// </summary>
public static class CatalogServiceHost
{
    public static WebApplication Build(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCatalog(builder.Configuration);
        builder.Services.AddScoped<CatalogWcfService>();
        builder.Services.AddServiceModelServices();
        builder.Services.AddServiceModelMetadata();
        builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();
        builder.Services.AddHealthChecks();

        var app = builder.Build();

        app.UseServiceModel(serviceModel =>
        {
            serviceModel.AddService<CatalogWcfService>(options => options.DebugBehavior.IncludeExceptionDetailInFaults =
                app.Environment.IsDevelopment());

            serviceModel.AddServiceEndpoint<CatalogWcfService, ICatalogWcfService>(
                new BasicHttpBinding(), ServiceAddresses.CatalogService);

            serviceModel.ConfigureServiceHostBase<CatalogWcfService>(host =>
                host.Description.Behaviors.Find<ServiceMetadataBehavior>().HttpGetEnabled = true);
        });

        app.MapHealthChecks("/health");

        if (app.Configuration.GetValue("Catalog:MigrateDatabaseOnStartup", true)
            && !app.Configuration.GetValue("Catalog:UseMockData", false))
        {
            using var scope = app.Services.CreateScope();
            scope.ServiceProvider.GetRequiredService<CatalogDbInitializer>().Initialize();
        }

        return app;
    }
}
