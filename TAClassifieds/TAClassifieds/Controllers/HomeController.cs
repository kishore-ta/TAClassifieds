using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TAClassifieds.Data;
using TAClassifieds.Model;

namespace TAClassifieds.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();            
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

         
        public ActionResult insertUser()
        {

            UnitOfWork uw = new UnitOfWork();
            var tab = uw.CategoryRepository.Get().ToList();

            //add category
            var cat = new Category() { CategoryName = "bla bla", CategoryImage = "img" };

            uw.CategoryRepository.Insert(cat);
            uw.Save();


            //UnitOfWork work = new UnitOfWork();
            //User user = new User();
            //user.First_Name = "Chaya krishna prasad";
            //user.Last_Name = "pothuraju Test";
            //user.Email = "ckrishnaprasad.pothuraju@techaspect.com";
            //work.UserRepository.Insert(user);

            //work.Save();

            return View("Index");
        }

        public bool sendMail(string userMail)
        {
            
            return true;
        }
    }
}

