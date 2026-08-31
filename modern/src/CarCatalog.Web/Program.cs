using CarCatalog.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddCatalog(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Catalog/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute("default", "{controller=Catalog}/{action=Index}/{id?}");

if (app.Configuration.GetValue("Catalog:MigrateDatabaseOnStartup", true)
    && !app.Configuration.GetValue("Catalog:UseMockData", false))
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<CatalogDbInitializer>().Initialize();
}

app.Run();

/// <summary>
/// Named entry point so the integration tests can host the app with WebApplicationFactory.
/// </summary>
public partial class Program;
