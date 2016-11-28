using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PersonalityPredictionAU.Models;
using System.IO;

namespace PersonalityPredictionAU.Controllers
{
    public class DictionariesController : Controller
    {
        private PersonalityPredictionDBEntities db = new PersonalityPredictionDBEntities();

        // GET: Dictionaries
        public ActionResult Index()
        {
            return View(db.Dictionaries.ToList());
        }

        // GET: Dictionaries/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dictionary dictionary = db.Dictionaries.Find(id);
            if (dictionary == null)
            {
                return HttpNotFound();
            }
            return View(dictionary);
        }

        // GET: Dictionaries/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Dictionaries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Date")] Dictionary dictionary)
        {
            if (ModelState.IsValid)
            {
                db.Dictionaries.Add(dictionary);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dictionary);
        }

        // GET: Dictionaries/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dictionary dictionary = db.Dictionaries.Find(id);
            if (dictionary == null)
            {
                return HttpNotFound();
            }
            return View(dictionary);
        }

        // POST: Dictionaries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Date")] Dictionary dictionary)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dictionary).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dictionary);
        }

        // GET: Dictionaries/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dictionary dictionary = db.Dictionaries.Find(id);
            if (dictionary == null)
            {
                return HttpNotFound();
            }
            return View(dictionary);
        }

        // POST: Dictionaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Dictionary dictionary = db.Dictionaries.Find(id);
            db.Dictionaries.Remove(dictionary);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            // make sure file isn't empty
            if(file.ContentLength > 0)
            {
                // Create context
                PersonalityPredictionDBEntities _context = new PersonalityPredictionDBEntities();
                // Set up dictionary to be created
                Dictionary dictionary = new Dictionary();
                dictionary.Name = "LiwcDictionary";
                dictionary.Date = DateTime.Now;
                _context.Dictionaries.Add(dictionary);
                // Set up stream reader to read through the file
                StreamReader sr = new StreamReader(file.InputStream);
                while (sr.Peek() != -1)
                {
                    // Split each line on commas and grab the first token as the category
                    string line = sr.ReadLine();
                    string[] tokens = line.Split(',');
                    int i = 0;
                    Category category = new Category();
                    category.Name = tokens[i];
                    category.Dictionary = dictionary;
                    _context.Categories.Add(category);
                    // This is the id for the word count category type
                    category.CategoryTypeId = 18; // You should probably change this :)
                    // Each token after that will be a word contained in that category
                    for (i = 1; i < tokens.Length; i++)
                    {
                        Word word = new Word();
                        word.Name = tokens[i];
                        category.Words.Add(word);
                        _context.Words.Add(word);
                    }
                }
                sr.Close();
                try
                {
                    _context.SaveChanges();
                }
                catch
                {
                    // add something here to let them know it broke.
                }
            }
            // didn't break
            return RedirectToAction("Index");
        }
    }
}
