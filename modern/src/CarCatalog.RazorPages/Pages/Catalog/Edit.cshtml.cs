using CarCatalog.Application;
using CarCatalog.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarCatalog.RazorPages.Pages.Catalog;

public class EditModel(ICatalogService service, ILogger<EditModel> logger) : PageModel
{
    [BindProperty]
    public CatalogItem Product { get; set; } = new();

    public SelectList Brands { get; private set; } = null!;

    public SelectList Types { get; private set; } = null!;

    public IActionResult OnGet(int id)
    {
        logger.LogInformation("Now loading... /Catalog/Edit?id={ProductId}", id);

        var product = service.FindCatalogItem(id);
        if (product == null)
        {
            return NotFound();
        }

        Product = product;
        PopulateSelectLists();
        return Page();
    }

    public IActionResult OnPost(int id)
    {
        Product.Id = id;

        if (!ModelState.IsValid)
        {
            PopulateSelectLists();
            return Page();
        }

        service.UpdateCatalogItem(Product);
        // The Web Forms pages redirected to the site root ("~") after a successful post.
        return Redirect("~/");
    }

    private void PopulateSelectLists()
    {
        Brands = new SelectList(service.GetCatalogBrands(), "Id", "Brand", Product.CatalogBrandId);
        Types = new SelectList(service.GetCatalogTypes(), "Id", "Type", Product.CatalogTypeId);
    }
}
