using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CarCatalog.Web.IntegrationTests;

public class CatalogPagesTests : IClassFixture<CatalogWebApplicationFactory>
{
    private static readonly Regex AntiForgeryField =
        new("name=\"__RequestVerificationToken\"[^>]*value=\"(?<token>[^\"]+)\"", RegexOptions.Compiled);

    private readonly CatalogWebApplicationFactory factory;

    public CatalogPagesTests(CatalogWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/Catalog/Index?pageSize=5&pageIndex=1")]
    [InlineData("/Catalog/Details/1")]
    [InlineData("/Catalog/Create")]
    [InlineData("/Catalog/Edit/1")]
    [InlineData("/Catalog/Delete/1")]
    public async Task Pages_RenderHtml(string url)
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Index_ListsTheSeededCatalog()
    {
        var client = factory.CreateClient();

        var html = await client.GetStringAsync("/");

        Assert.Contains("Velocari Strada SV", html);
        Assert.Contains("Showing 10 of 12 products", html);
    }

    [Fact]
    public async Task Details_ReturnsNotFoundForUnknownItem()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/Catalog/Details/4242");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Pic_ServesThePictureOfAnItem()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/items/1/pic");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("image/png", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Brands_ApiReturnsTheBrands()
    {
        var client = factory.CreateClient();

        var brands = await client.GetFromJsonAsync<List<BrandResponse>>("/api/brands");

        Assert.NotNull(brands);
        Assert.Equal("Velocari", brands![0].Brand);
    }

    [Fact]
    public async Task Create_PostRedirectsToIndexAndStoresTheItem()
    {
        // A dedicated host: the in-memory catalog is a singleton, so writes must not leak into the
        // tests that assert on the seeded catalog.
        using var isolated = new CatalogWebApplicationFactory();
        var client = isolated.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var form = await GetAntiForgeryFormAsync(client, "/Catalog/Create");
        form.Fields["Name"] = "Integration Test Car";
        form.Fields["Description"] = "Created by a test";
        form.Fields["Price"] = "1000";
        form.Fields["PictureFileName"] = "1.png";
        form.Fields["CatalogBrandId"] = "1";
        form.Fields["CatalogTypeId"] = "1";
        form.Fields["AvailableStock"] = "1";
        form.Fields["RestockThreshold"] = "0";
        form.Fields["MaxStockThreshold"] = "10";

        var response = await client.PostAsync("/Catalog/Create", new FormUrlEncodedContent(form.Fields));

        Assert.Equal(HttpStatusCode.Found, response.StatusCode);
        Assert.StartsWith("/", response.Headers.Location?.OriginalString);

        var lastPage = await client.GetStringAsync("/Catalog/Index?pageSize=10&pageIndex=1");
        Assert.Contains("Integration Test Car", lastPage);
    }

    [Fact]
    public async Task Create_PostWithoutNameRedisplaysTheFormWithAValidationError()
    {
        var client = factory.CreateClient();

        var form = await GetAntiForgeryFormAsync(client, "/Catalog/Create");
        form.Fields["Name"] = string.Empty;
        form.Fields["CatalogBrandId"] = "1";
        form.Fields["CatalogTypeId"] = "1";

        var response = await client.PostAsync("/Catalog/Create", new FormUrlEncodedContent(form.Fields));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("The Name field is required.", await response.Content.ReadAsStringAsync());
    }

    private static async Task<AntiForgeryForm> GetAntiForgeryFormAsync(HttpClient client, string url)
    {
        var html = await client.GetStringAsync(url);
        var match = AntiForgeryField.Match(html);
        Assert.True(match.Success, $"No antiforgery token found on {url}.");

        return new AntiForgeryForm(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = match.Groups["token"].Value,
        });
    }

    private sealed record AntiForgeryForm(Dictionary<string, string> Fields);

    private sealed record BrandResponse(int Id, string Brand);
}
