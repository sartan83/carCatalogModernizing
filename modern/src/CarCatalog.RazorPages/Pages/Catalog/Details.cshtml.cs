using CarCatalog.Application;
using CarCatalog.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarCatalog.RazorPages.Pages.Catalog;

public class DetailsModel(ICatalogService service, ILogger<DetailsModel> logger) : PageModel
{
    public CatalogItem Product { get; private set; } = null!;

    public IActionResult OnGet(int id)
    {
        logger.LogInformation("Now loading... /Catalog/Details?id={ProductId}", id);

        var product = service.FindCatalogItem(id);
        if (product == null)
        {
            return NotFound();
        }

        Product = product;
        return Page();
    }
}
