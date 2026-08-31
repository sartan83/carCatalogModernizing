using System.Web.Mvc;

namespace CarCatalogLegacyMVC.Controllers.Api
{
    [Route("api")]
    public class CatalogController2 : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return Json(new { Message = "Hello World!" });
        }
    }
}
