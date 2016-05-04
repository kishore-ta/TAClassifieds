using System;
using System.Collections.Generic;
using System.IO;
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
        public ActionResult PostAdClassifiedVM(ClassifiedContactVM Model, string categoryvalue, HttpPostedFileBase file)
        {
            try
            {
                var path = string.Empty;
                UnitOfWork uw = new UnitOfWork();

                Classified obj = new Classified();
                obj.CategoryId = Convert.ToInt16(categoryvalue);
                obj.ClassifiedTitle = Model.ClassifiedTitle;
                obj.Description = Model.Description;

                if (file != null && file.ContentLength > 0)
                {
                    // extract only the filename
                    var fileName = Path.GetFileName(file.FileName);
                    // store the file inside ~/BrowseImages folder
                    path = Server.MapPath("~/BrowseImages/" + Guid.NewGuid() + "-" + fileName);
                    file.SaveAs(path);
                    obj.ClassifiedImage = "~/BrowseImages/" + Guid.NewGuid() + "-" + fileName;
                }

                obj.ClassifiedPrice = Convert.ToInt32(Model.ClassifiedPrice);
                obj.Summary = Model.Description;
                obj.PostedDate = DateTime.Now;
                obj.CreatedBy = Guid.Parse("aa968550-1c9e-483b-95d5-c12eab243024");

                ClassifiedContact obj1 = Model.classifiedsContacts;

                Classified cls = uw.ClassifiedRepository.Insert(obj);
                obj1.ClassifiedId = cls.ClassifiedId;

                uw.ClassifiedContactRepository.Insert(obj1);
                uw.Save();
            }
            catch (Exception ex)
            {
            }
            return RedirectToAction("Index", "Home");
        }
    }
}