using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TAClassifieds.Data;

namespace TAClassifieds.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
            //test
            //test check-in
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        [HttpGet]
        public ActionResult insertUser(string fn,string ln,string email)
        {
            return View();
        }
         
        public ActionResult insertUser()
        {
            using (var db = new TAC_Team1Entities())
            {
                TAC_User user = new TAC_User();
                user.First_Name = "Chaya krishna prasad";
                user.Last_Name = "pothuraju";
                user.Email = "ckrishnaprasad.pothuraju@techaspect.com";
                db.TAC_User.Add(user);
                db.SaveChanges();
            }
            return View("Index");
        }

        public bool sendMail(string userMail)
        {
            
            return true;
        }
    }
}

