using CarCatalog.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarCatalog.Infrastructure;

public static class CatalogServiceCollectionExtensions
{
    /// <summary>
    /// Registers the catalog service. With <c>Catalog:UseMockData</c> set the in-memory catalog is
    /// used and no database is touched, mirroring the legacy <c>UseMockData</c> app setting.
    /// </summary>
    public static IServiceCollection AddCatalog(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.GetValue("Catalog:UseMockData", false))
        {
            services.AddSingleton<ICatalogService, InMemoryCatalogService>();
            return services;
        }

        services.AddDbContext<CatalogDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("Catalog"),
                sql => sql.EnableRetryOnFailure()));

        services.AddScoped<ICatalogService, EfCatalogService>();
        services.AddScoped<CatalogDbInitializer>();

        return services;
    }
}
