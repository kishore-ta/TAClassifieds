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
            
            Model.categoriesList = uw.CategoryRepository.Get().ToList();         


            return View(Model);
        }

        [HttpPost]
        [ActionName("PostAd")]
        public ActionResult PostAdClassifiedVM(ClassifiedContactVM Model)
        {
            UnitOfWork uw = new UnitOfWork();
            
            int categoryId = 2;
            // var user1 = uw.UserRepository.GetByID(Guid.Parse("aa968550-1c9e-483b-95d5-c12eab243024"));

            Classified obj = new Classified();
            obj.CategoryId = Model.CategoryId;
            obj.ClassifiedTitle = Model.ClassifiedTitle;
            obj.Description = Model.Description;
            obj.ClassifiedImage = Model.ClassifiedImage;
            obj.ClassifiedPrice = Convert.ToInt32(Model.ClassifiedPrice);
            obj.Summary = Model.Description;
            obj.PostedDate = DateTime.Now;
            obj.CreatedBy = Guid.Parse("aa968550-1c9e-483b-95d5-c12eab243024");
            //obj.User=user1;
            ClassifiedContact obj1 = Model.classifiedsContacts;
            
            Classified cls = uw.ClassifiedRepository.Insert(obj);
            obj1.ClassifiedId = cls.ClassifiedId;

            //obj.ClassifiedContacts.Add(obj1);

            uw.ClassifiedContactRepository.Insert(obj1);

            uw.Save();
           
            return View(Model);
        }
    }
}