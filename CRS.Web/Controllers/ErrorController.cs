using System.Web.Mvc;

namespace CRS.Web.Controllers
{
    public class ErrorController : FrontEndControllerBase
    {
        //
        // GET: /Error/

        public ActionResult Index()
        {
            return View();
        }

    }
}
