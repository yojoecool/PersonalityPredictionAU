using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PersonalityPredictionAU.Models;

namespace PersonalityPredictionAU.Controllers
{
    public class DictionaryController : Controller
    {

        private PersonalityPredictionDBEntities _context;

        public DictionaryController()
        {
            _context = new PersonalityPredictionDBEntities();
        }

        // GET: Dictionary
        public ActionResult Index()
        {
            IEnumerable<Dictionary> Dictionaries = _context.Dictionaries;
            return View(Dictionaries);
        }

        // GET: Dictionary/Details/5
        public ActionResult Details(int id)
        {
            Dictionary dict = _context.Dictionaries.Find(id);
            return View(dict);
        }

        // GET: Dictionary/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Dictionary/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                Dictionary dict = new Dictionary()
                {
                    Name = collection["Name"],
                    Date = DateTime.Now
                };
                _context.Dictionaries.Add(dict);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Dictionary/Edit/5
        public ActionResult Edit(int id)
        {
            Dictionary dict = _context.Dictionaries.Find(id);
            return View(dict);
        }

        // POST: Dictionary/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                Dictionary dict = _context.Dictionaries.Find(id);
                dict.Name = collection["Name"];
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Dictionary/Delete/5
        public ActionResult Delete(int id)
        {
            Dictionary dict = _context.Dictionaries.Find(id);
            return View(dict);
        }

        // POST: Dictionary/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                Dictionary dict = _context.Dictionaries.Find(id);
                _context.Dictionaries.Remove(dict);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
