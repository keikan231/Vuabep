using System.Web.Mvc;

namespace CRS.Web.Areas.Admin.Controllers
{
    public class AdminHomeController : AdminControllerBase
    {
        //
        // GET: /Admin/AdminHome/

        public ActionResult Index()
        {
            return View();
        }

    }
}
