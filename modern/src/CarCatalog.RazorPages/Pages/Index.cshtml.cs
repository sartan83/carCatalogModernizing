using CarCatalog.Application;
using CarCatalog.Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarCatalog.RazorPages.Pages;

public class IndexModel(ICatalogService service, ILogger<IndexModel> logger) : PageModel
{
    public const int DefaultPageIndex = 0;
    public const int DefaultPageSize = 10;

    public PaginatedItems<CatalogItem> Catalog { get; private set; } = null!;

    public void OnGet(int? index, int? size)
    {
        var pageIndex = index ?? DefaultPageIndex;
        var pageSize = size ?? DefaultPageSize;

        logger.LogInformation("Now loading... /Default?size={PageSize}&index={PageIndex}", pageSize, pageIndex);

        Catalog = service.GetCatalogItemsPaginated(pageSize, pageIndex);
    }
}
