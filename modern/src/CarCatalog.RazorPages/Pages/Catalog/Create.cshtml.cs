using CarCatalog.Application;
using CarCatalog.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarCatalog.RazorPages.Pages.Catalog;

public class CreateModel(ICatalogService service, ILogger<CreateModel> logger) : PageModel
{
    [BindProperty]
    public CatalogItem Product { get; set; } = new();

    public SelectList Brands { get; private set; } = null!;

    public SelectList Types { get; private set; } = null!;

    public void OnGet()
    {
        logger.LogInformation("Now loading... /Catalog/Create");
        PopulateSelectLists();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            PopulateSelectLists();
            return Page();
        }

        service.CreateCatalogItem(Product);
        // The Web Forms pages redirected to the site root ("~") after a successful post.
        return Redirect("~/");
    }

    private void PopulateSelectLists()
    {
        Brands = new SelectList(service.GetCatalogBrands(), "Id", "Brand", Product.CatalogBrandId);
        Types = new SelectList(service.GetCatalogTypes(), "Id", "Type", Product.CatalogTypeId);
    }
}
