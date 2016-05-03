using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TAClassifieds.Model;

namespace TAClassifieds.Controllers
{
    public class PostAdController : Controller
    {
        //
        // GET: /PostAd/
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult PostAd(ClassifiedContactVM Model)
        {
            UnitOfWork uw = new UnitOfWork();
            //var categoriesList = uw.CategoryRepository.Get().ToList();
            Model.categoriesList = uw.CategoryRepository.Get().ToList();
            return View(Model);
        }

        [HttpPost]
        [ActionName("PostAd")]
        public ActionResult PostAdClassifiedVM(ClassifiedContactVM Model)
        {
            //    UnitOfWork uw = new UnitOfWork();
            //    var clsfied = new Classified() {CategoryName = "bla bla", CategoryImage = "img"};

            //    uw.ClassifiedRepository.Insert(clsfied);
            //    uw.Save();

            //    //Model.categoriesList = uw.CategoryRepository.Get().ToList();
            return View(Model);
        }
	}
}