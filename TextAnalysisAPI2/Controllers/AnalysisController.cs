using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace TextAnalysisAPI.Controllers
{
    public class AnalysisController : ApiController
    {
        private PersonalityPredictionDBEntities _context = new PersonalityPredictionDBEntities();
        // GET: api/Analysis
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        public class CategoryScore
        {
            public string CategoryName { get; set; }
            public float Score { get; set; }
        }

        // GET: api/Analysis/5
        [Authorize]
        public JsonResult<List<CategoryScore>> Get(string text)
        {
            Dictionary dictionary = _context.Dictionaries.Find(3);
            string[] words = text.Split(' ');
            List<CategoryScore> scores = new List<CategoryScore>();
            int totalCount = words.Count();
            foreach (Category category in dictionary.Categories.Where(c => c.CategoryType.Name == "WordFrequency"))
            {
                int categoryCount = 0;
                foreach (string word in words)
                {
                    if(category.Words.Any(w => w.Name == word))
                    {
                        categoryCount++;
                    }
                }
                scores.Add(new CategoryScore()
                {
                    CategoryName = category.Name,
                    Score = (float)categoryCount / (float)totalCount
                });

            }
            return Json(scores);
        }

        // POST: api/Analysis
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Analysis/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Analysis/5
        public void Delete(int id)
        {
        }
    }
}
