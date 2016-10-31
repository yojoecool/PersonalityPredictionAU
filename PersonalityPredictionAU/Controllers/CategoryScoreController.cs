using PersonalityPredictionAU.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace PersonalityPredictionAU.Controllers
{
    public class CategoryScoreController : Controller
    {
        private PersonalityPredictionDBEntities _context = new PersonalityPredictionDBEntities();

        // GET: CategoryScore
        public ActionResult Index()
        {
            return View();
        }

        // GET: CategoryScore/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CategoryScore/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoryScore/Create
        [HttpPost]
        public String Create(List<LiwcScoreModel> Scores)
        {
            try
            {
                // Find the user in the account table
                string username = User.Identity.Name;
                Account user = _context.Accounts.FirstOrDefault(a => a.Email == username);

                // Remove previous scores
                foreach(CategoryScore catScore in user.CategoryScores.ToList())
                {
                    _context.CategoryScores.Remove(catScore);
                }

                // Add in new scores
                foreach(LiwcScoreModel score in Scores)
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

        // GET: CategoryScore/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CategoryScore/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: CategoryScore/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CategoryScore/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
