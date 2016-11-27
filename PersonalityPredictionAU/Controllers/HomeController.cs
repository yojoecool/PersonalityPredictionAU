using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PersonalityPredictionAU.Models;
using System.Web.Script.Serialization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;

namespace PersonalityPredictionAU.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private static HttpClient client = null;
        private PersonalityPredictionDBEntities _context = new PersonalityPredictionDBEntities();

        public ActionResult Index()
        {
            int id = -1;
            if (User.Identity.IsAuthenticated)
            {
                id = new PersonalityPredictionDBEntities().Accounts.FirstOrDefault(a => a.Email == User.Identity.Name).Id;
            }
            ViewBag.id = id;
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
                int id = _context.Accounts.FirstOrDefault(a => a.Email == User.Identity.Name).Id;
                ViewBag.id = id;
                return View();
            }
            else return RedirectToAction("UnAuthenticated");
        }

        public ActionResult UnAuthenticated()
        {
            return View();
        }

        private String Create(List<LiwcScoreModel> Scores)
        {
            try
            {
                // Find the user in the account table
                string username = User.Identity.Name;
                Account user = _context.Accounts.FirstOrDefault(a => a.Email == username);

                // Remove previous scores
                foreach (CategoryScore catScore in user.CategoryScores.ToList())
                {
                    _context.CategoryScores.Remove(catScore);
                }

                // Add in new scores
                foreach (LiwcScoreModel score in Scores)
                {
                    _context.CategoryScores.Add(new CategoryScore()
                    {
                        Account = user,
                        Category = _context.Categories.FirstOrDefault(c => c.Name == score.CategoryName),
                        SourceId = 1,
                        Score = score.Score
                    });
                }

                // Save changes to database
                _context.SaveChanges();
                //return RedirectToAction("Index");
                return "Success";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //return View();
                return "Fail";
            }
        }

        [HttpPost]
        public async Task<String> AnalyzeText(string text)
        {
            String PostUrl = "http://textanalysisapi.azurewebsites.net/api/TextAnalysis";
            if (client == null)
            {
                client = new HttpClient();
                client.BaseAddress = new Uri("http://textanalysisapi.azurewebsites.net");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }

            HttpContent content = new StringContent(JsonConvert.SerializeObject(text), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(PostUrl, content).Result;
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Success");
                List<LiwcScoreModel> result = JsonConvert.DeserializeObject<List<LiwcScoreModel>>(await response.Content.ReadAsStringAsync());
                String success = Create(result);
                return success;
            }
            else
            {
                Console.WriteLine("Fail");
                Console.WriteLine(response.StatusCode);
                return "Fail";
            }
        }

        public ActionResult FileUpload()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
                return RedirectToAction("UnAuthenticated");
        }

        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase file)
        {
            string result = new System.IO.StreamReader(file.InputStream).ReadToEnd();

            string x = retrieveScores(result);

            if (x != null)
            {
                int id = _context.Accounts.FirstOrDefault(a => a.Email == User.Identity.Name).Id;
                return RedirectToAction("Details", "Users", new { id = id });
            }
            else return RedirectToAction("Failed");
        }

        public ActionResult Failed()
        {
            return View();
        }

        private String retrieveScores(String content)
        {
            // Keys for accessing API
            String url = "https://textanalysisapi.azurewebsites.net/api/textanalysis";

            // Setting up the request to the API
            // TODO Add exception handling for 400,404,etc. responses
            WebClient request = new WebClient();
            request.Headers.Add("Content-Type", "application/json");
            String responseText = request.UploadString(url, JsonConvert.SerializeObject(content));

            // Interpret the JSON response
            JArray a = JArray.Parse(responseText);
            List<LiwcScoreModel> Scores = new List<LiwcScoreModel>();
            foreach (JObject obj in a.Children<JObject>())
            {
                Scores.Add(obj.ToObject<LiwcScoreModel>());
            }

            this.Create(Scores);

            return responseText;
        }

        public ActionResult Terms()
        {
            return View();
        }

        public ActionResult PrivacyPolicy()
        {
            return View();
        }
    }
}