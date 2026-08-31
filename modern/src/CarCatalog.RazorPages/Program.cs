using CarCatalog.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options =>
{
    // The Web Forms app mapped these page routes in App_Start/RouteConfig.cs.
    options.Conventions.AddPageRoute("/Index", "Default");
    options.Conventions.AddPageRoute("/Index", "Default/index/{index:int}/size/{size:int}");
});
builder.Services.AddCatalog(builder.Configuration);
builder.Services.AddCatalogHealthChecks(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();
app.MapHealthChecks("/health");

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
