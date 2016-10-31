using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PersonalityPredictionAU.Models;
using System.Web.Script.Serialization;
using System.Net;

namespace PersonalityPredictionAU.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private PersonalityPredictionDBEntities _context = new PersonalityPredictionDBEntities();

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

        [HttpPost]
        public String retrieveScores(String content)
        {
            // Keys for accessing API
            String api_key = "57f4699a7ba60a05aad03fcf";
            String api_secret_key = "oJ1bk5DlRmDkzneGKvr459Zs56Z1DNrEzyrpdnlhCS0";
            String url = "https://textanalysisapi.azurewebsites.net/api/textanalysis?text=\"" + content + "\"";

            // Setting up the request to the API
            // TODO Add exception handling for 400,404,etc. responses
            WebClient request = new WebClient();
            //request.Headers.Add("Content-Type", "application/json");
            //request.Headers.Add("X-API-KEY", api_key);
            //request.Headers.Add("X-API-SECRET-KEY", api_secret_key);
            //byte[] responseArray = request.UploadData("https://app.receptiviti.com/api/person", "POST", System.Text.Encoding.ASCII.GetBytes(content));
            byte[] responseArray = request.DownloadData(url);

            // Preprocess the response to target the scores
            String responseText = System.Text.Encoding.ASCII.GetString(responseArray);
            int start = responseText.IndexOf("\"liwc_scores\"");
            int end = responseText.IndexOf("\"content_date\"");
            responseText = responseText.Substring(start + 16, end - start - 20);

            // Iteratively retrieve the scores
            List<LiwcScoreModel> Scores = new List<LiwcScoreModel>();
            while (responseText.Length > 0)
            {
                LiwcScoreModel mod = new LiwcScoreModel();
                int subStart = responseText.IndexOf("\"") + 1;  // Each category is delimited by quotes
                int subEnd = responseText.IndexOf(":") - 1;     // The : is a good landmark for the end of the category
                mod.Category = responseText.Substring(subStart, subEnd - subStart); // Retrieve the Category
                if (mod.Category == "categories")   // This entry contains a list of the individual, non-aggregate scores
                {
                    responseText = responseText.Substring(subEnd + 4);  // Repurpose the string to ignore the "categories" label and parse the scores again
                    continue;
                }
                subStart = subEnd + 3;  // Move subStart to the beginning of the score itself
                subEnd = (responseText.Contains(",") ? responseText.IndexOf(",") : responseText.Length);    // Find the end of the score
                mod.Score = (float) Convert.ToDouble(responseText.Substring(subStart, subEnd - subStart));  // Save the score
                Scores.Add(mod);
                responseText = (responseText.Length == subEnd ? "" : responseText.Substring(subEnd + 1));   // Progress the response text to the next category
            }

            this.addToDB(Scores);
            
            return responseText;
        }

        private void addToDB(List<LiwcScoreModel> Scores)
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
                        Category = _context.Categories.FirstOrDefault(c => c.Name == score.Category),
                        SourceId = 1,
                        Score = score.Score
                    });
                }

                // Save changes to database
                _context.SaveChanges();
                return;
            }
            catch
            {
                return;
            }
        }
    }
}