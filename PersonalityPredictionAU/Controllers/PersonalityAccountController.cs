using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PersonalityPredictionAU.Models;

namespace PersonalityPredictionAU.Controllers
{
    public class PersonalityAccountController : Controller
    {
        private PersonalityPredictionDBEntities _context;

        public PersonalityAccountController()
        {
            _context = new PersonalityPredictionDBEntities();
        }

        // GET: PersonalityAccount
        public ActionResult Index()
        {
            IEnumerable<Account> users = _context.Accounts;
            return View(users);
        }


        // GET: PersonalityAccount
        public ActionResult Details(int id)
        {
            Account user = _context.Accounts.Find(id);
            return View(user);
        }
    }
}