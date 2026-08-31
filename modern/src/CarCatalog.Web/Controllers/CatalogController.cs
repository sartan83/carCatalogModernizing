using CarCatalog.Application;
using CarCatalog.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarCatalog.Web.Controllers;

public class CatalogController : Controller
{
    private readonly ICatalogService service;
    private readonly ILogger<CatalogController> logger;

    public CatalogController(ICatalogService service, ILogger<CatalogController> logger)
    {
        this.service = service;
        this.logger = logger;
    }

    // GET /[?pageSize=3&pageIndex=10]
    public IActionResult Index(int pageSize = 10, int pageIndex = 0)
    {
        logger.LogInformation("Now loading... /Catalog/Index?pageSize={PageSize}&pageIndex={PageIndex}", pageSize, pageIndex);

        var paginatedItems = service.GetCatalogItemsPaginated(pageSize, pageIndex);
        foreach (var item in paginatedItems.Data)
        {
            AddPictureUri(item);
        }

        return View(paginatedItems);
    }

    // GET: Catalog/Details/5
    public IActionResult Details(int? id)
    {
        if (id == null)
        {
            return BadRequest();
        }

        var catalogItem = service.FindCatalogItem(id.Value);
        if (catalogItem == null)
        {
            return NotFound();
        }

        AddPictureUri(catalogItem);
        return View(catalogItem);
    }

    // GET: Catalog/Create
    public IActionResult Create()
    {
        PopulateSelectLists();
        return View(new CatalogItem());
    }

    // POST: Catalog/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind(nameof(CatalogItem.Name), nameof(CatalogItem.Description), nameof(CatalogItem.Price), nameof(CatalogItem.PictureFileName), nameof(CatalogItem.CatalogTypeId), nameof(CatalogItem.CatalogBrandId), nameof(CatalogItem.AvailableStock), nameof(CatalogItem.RestockThreshold), nameof(CatalogItem.MaxStockThreshold), nameof(CatalogItem.OnReorder))] CatalogItem catalogItem)
    {
        if (ModelState.IsValid)
        {
            service.CreateCatalogItem(catalogItem);
            return RedirectToAction(nameof(Index));
        }

        PopulateSelectLists(catalogItem);
        return View(catalogItem);
    }

    // GET: Catalog/Edit/5
    public IActionResult Edit(int? id)
    {
        if (id == null)
        {
            return BadRequest();
        }

        var catalogItem = service.FindCatalogItem(id.Value);
        if (catalogItem == null)
        {
            return NotFound();
        }

        AddPictureUri(catalogItem);
        PopulateSelectLists(catalogItem);
        return View(catalogItem);
    }

    // POST: Catalog/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit([Bind(nameof(CatalogItem.Id), nameof(CatalogItem.Name), nameof(CatalogItem.Description), nameof(CatalogItem.Price), nameof(CatalogItem.PictureFileName), nameof(CatalogItem.CatalogTypeId), nameof(CatalogItem.CatalogBrandId), nameof(CatalogItem.AvailableStock), nameof(CatalogItem.RestockThreshold), nameof(CatalogItem.MaxStockThreshold), nameof(CatalogItem.OnReorder))] CatalogItem catalogItem)
    {
        if (ModelState.IsValid)
        {
            service.UpdateCatalogItem(catalogItem);
            return RedirectToAction(nameof(Index));
        }

        AddPictureUri(catalogItem);
        PopulateSelectLists(catalogItem);
        return View(catalogItem);
    }

    // GET: Catalog/Delete/5
    public IActionResult Delete(int? id)
    {
        if (id == null)
        {
            return BadRequest();
        }

        var catalogItem = service.FindCatalogItem(id.Value);
        if (catalogItem == null)
        {
            return NotFound();
        }

        AddPictureUri(catalogItem);
        return View(catalogItem);
    }

    // POST: Catalog/Delete/5
    [HttpPost]
    [ActionName(nameof(Delete))]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var catalogItem = service.FindCatalogItem(id);
        if (catalogItem == null)
        {
            return NotFound();
        }

        service.RemoveCatalogItem(catalogItem);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Error() => View();

    private void PopulateSelectLists(CatalogItem? selected = null)
    {
        ViewBag.CatalogBrandId = new SelectList(service.GetCatalogBrands(), "Id", "Brand", selected?.CatalogBrandId);
        ViewBag.CatalogTypeId = new SelectList(service.GetCatalogTypes(), "Id", "Type", selected?.CatalogTypeId);
    }

    private void AddPictureUri(CatalogItem item)
    {
        item.PictureUri = Url.RouteUrl(PicController.GetPicRouteName, new { catalogItemId = item.Id }, Request.Scheme);
    }
}
