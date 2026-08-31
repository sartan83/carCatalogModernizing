using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CarCatalog.RazorPages.IntegrationTests;

public class CatalogPagesTests(CatalogPagesApplicationFactory factory) : IClassFixture<CatalogPagesApplicationFactory>
{
    private static readonly Regex AntiForgeryField =
        new("name=\"__RequestVerificationToken\"[^>]*value=\"(?<token>[^\"]+)\"", RegexOptions.Compiled);

    [Theory]
    [InlineData("/")]
    [InlineData("/Default")]
    [InlineData("/Default/index/1/size/5")]
    [InlineData("/Catalog/Create")]
    [InlineData("/Catalog/Details/1")]
    [InlineData("/Catalog/Edit/1")]
    [InlineData("/Catalog/Delete/1")]
    public async Task Pages_RenderHtml(string url)
    {
        var response = await factory.CreateClient().GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Index_ListsTheSeededCatalog()
    {
        var html = await factory.CreateClient().GetStringAsync("/");

        Assert.Contains("Velocari Strada SV", html);
        Assert.Contains("Showing 10 of 12 products", html);
    }

    [Fact]
    public async Task Index_HonoursThePaginationRoute()
    {
        var html = await factory.CreateClient().GetStringAsync("/Default/index/1/size/5");

        Assert.Contains("Showing 5 of 12 products - Page 2 - 3", html);
    }

    [Fact]
    public async Task Details_ReturnsNotFoundForUnknownItem()
    {
        var response = await factory.CreateClient().GetAsync("/Catalog/Details/4242");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Pics_AreServedAsStaticFiles()
    {
        var response = await factory.CreateClient().GetAsync("/Pics/1.png");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("image/png", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Create_PostRedirectsToIndexAndStoresTheItem()
    {
        // A dedicated host: the in-memory catalog is a singleton, so writes must not leak into the
        // tests that assert on the seeded catalog.
        using var isolated = new CatalogPagesApplicationFactory();
        var client = isolated.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var fields = await GetAntiForgeryFieldsAsync(client, "/Catalog/Create");
        fields["Product.Name"] = "Integration Test Car";
        fields["Product.Description"] = "Created by a test";
        fields["Product.Price"] = "1000";
        fields["Product.CatalogBrandId"] = "1";
        fields["Product.CatalogTypeId"] = "1";
        fields["Product.AvailableStock"] = "1";
        fields["Product.RestockThreshold"] = "0";
        fields["Product.MaxStockThreshold"] = "10";

        var response = await client.PostAsync("/Catalog/Create", new FormUrlEncodedContent(fields));

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/", response.Headers.Location?.OriginalString);
        Assert.Contains("Integration Test Car", await client.GetStringAsync("/Default/index/1/size/10"));
    }

    [Fact]
    public async Task Create_PostRedisplaysThePageWhenValidationFails()
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var fields = await GetAntiForgeryFieldsAsync(client, "/Catalog/Create");
        fields["Product.Price"] = "1000";

        var response = await client.PostAsync("/Catalog/Create", new FormUrlEncodedContent(fields));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("The Name field is required.", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Delete_PostRemovesTheItem()
    {
        using var isolated = new CatalogPagesApplicationFactory();
        var client = isolated.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var fields = await GetAntiForgeryFieldsAsync(client, "/Catalog/Delete/1");

        var response = await client.PostAsync("/Catalog/Delete/1", new FormUrlEncodedContent(fields));

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/Catalog/Details/1")).StatusCode);
    }

    private static async Task<Dictionary<string, string>> GetAntiForgeryFieldsAsync(HttpClient client, string url)
    {
        var html = await client.GetStringAsync(url);
        var match = AntiForgeryField.Match(html);
        Assert.True(match.Success, $"No antiforgery token found on {url}.");

        return new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = match.Groups["token"].Value,
        };
    }
}
