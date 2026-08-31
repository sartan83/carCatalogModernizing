using CarCatalog.Application;
using CarCatalog.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarCatalog.RazorPages.Pages.Catalog;

public class DeleteModel(ICatalogService service, ILogger<DeleteModel> logger) : PageModel
{
    public CatalogItem ProductToDelete { get; private set; } = null!;

    public IActionResult OnGet(int id)
    {
        logger.LogInformation("Now loading... /Catalog/Delete?id={ProductId}", id);

        var product = service.FindCatalogItem(id);
        if (product == null)
        {
            return NotFound();
        }

        ProductToDelete = product;
        return Page();
    }

    public IActionResult OnPost(int id)
    {
        var product = service.FindCatalogItem(id);
        if (product == null)
        {
            return NotFound();
        }

        service.RemoveCatalogItem(product);
        // The Web Forms pages redirected to the site root ("~") after a successful post.
        return Redirect("~/");
    }
}
