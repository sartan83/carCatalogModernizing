using CarCatalog.Application;
using CarCatalog.Domain;
using Microsoft.AspNetCore.Mvc;

namespace CarCatalog.Web.Controllers;

[ApiController]
[Route("api/brands")]
public class BrandsController : ControllerBase
{
    private readonly ICatalogService service;

    public BrandsController(ICatalogService service)
    {
        this.service = service;
    }

    [HttpGet]
    public IEnumerable<CatalogBrand> Get() => service.GetCatalogBrands();

    [HttpGet("{id:int}")]
    public ActionResult<CatalogBrand> Get(int id)
    {
        var brand = service.GetCatalogBrands().FirstOrDefault(b => b.Id == id);

        return brand == null ? NotFound() : brand;
    }
}
