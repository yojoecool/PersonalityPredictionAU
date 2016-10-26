using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TextAnalysisAPI.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.Title = "Not Authenticated";
            }

            return View();
        }
    }
}
