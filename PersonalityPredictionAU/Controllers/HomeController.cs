using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PersonalityPredictionAU.Models;
using System.Web.Script.Serialization;

namespace PersonalityPredictionAU.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Personality Prediction";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult PostRetrieval()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else return RedirectToAction("UnAuthenticated");
        }

        public ActionResult UnAuthenticated()
        {
            return View();
        }

        [HttpPost]
        public String userPostsTxt(String response, String userID)
        {
            //string fileName = System.IO.Path.GetTempPath() + userID + "_posts.txt";
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "Temp\\" + userID + "_posts.txt";
            //String allPosts = new JavaScriptSerializer().Deserialize<String>(response);
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(System.IO.File.Open(fileName, System.IO.FileMode.Create)))
            {
                file.Write(response);
            }
            return "/Temp/" + userID + "_posts.txt";
        }
    }
}