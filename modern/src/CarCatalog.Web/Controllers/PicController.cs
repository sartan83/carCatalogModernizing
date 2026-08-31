using CarCatalog.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CarCatalog.Web.Controllers;

public class PicController : Controller
{
    public const string GetPicRouteName = "GetPicRouteTemplate";

    private static readonly FileExtensionContentTypeProvider ContentTypes = new();

    private readonly ICatalogService service;
    private readonly IWebHostEnvironment environment;

    public PicController(ICatalogService service, IWebHostEnvironment environment)
    {
        this.service = service;
        this.environment = environment;
    }

    // GET: items/5/pic
    [HttpGet]
    [Route("items/{catalogItemId:int}/pic", Name = GetPicRouteName)]
    public IActionResult Index(int catalogItemId)
    {
        if (catalogItemId <= 0)
        {
            return BadRequest();
        }

        var item = service.FindCatalogItem(catalogItemId);
        if (item == null)
        {
            return NotFound();
        }

        var path = Path.Combine(environment.WebRootPath, "pics", item.PictureFileName);
        if (!System.IO.File.Exists(path))
        {
            return NotFound();
        }

        if (!ContentTypes.TryGetContentType(path, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        return PhysicalFile(path, contentType);
    }
}
