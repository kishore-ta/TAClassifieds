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
            try
            {
                UnitOfWork uw = new UnitOfWork();
                Model.categoriesList = uw.CategoryRepository.Get().ToList();
            }
            catch (Exception ex)
            {

            }
            return View(Model);
        }

        [HttpPost]
        [ActionName("PostAd")]
        public ActionResult PostAdClassifiedVM(ClassifiedContactVM Model, string categoryvalue, HttpPostedFileBase file)
        {
            try
            {
                var path = string.Empty;
                var strPath = string.Empty;
                UnitOfWork uw = new UnitOfWork();

                Classified objClassified = new Classified();
                objClassified.CategoryId = Convert.ToInt16(categoryvalue);
                objClassified.ClassifiedTitle = Model.ClassifiedTitle;
                objClassified.Description = Model.Description;

                if (file != null && file.ContentLength > 0)
                {
                    // extract only the filename
                    var fileName = Path.GetFileName(file.FileName);
                    // store the file inside ~/BrowseImages folder
                    strPath = "/BrowseImages/" + Guid.NewGuid() + "-" + fileName;
                    path = Server.MapPath(strPath);
                    file.SaveAs(path);
                    objClassified.ClassifiedImage = strPath;
                }

                objClassified.ClassifiedPrice = Convert.ToInt32(Model.ClassifiedPrice);
                objClassified.Summary = Model.Description;
                objClassified.PostedDate = DateTime.Now;
                objClassified.CreatedBy = Guid.Parse("aa968550-1c9e-483b-95d5-c12eab243024");

                ClassifiedContact objContact = Model.classifiedsContacts;

                Classified cls = uw.ClassifiedRepository.Insert(objClassified);
                objContact.ClassifiedId = cls.ClassifiedId;

                uw.ClassifiedContactRepository.Insert(objContact);
                uw.Save();
            }
            catch(Exception ex)
            {

            }
            return RedirectToAction("GetAd", "Home");
        }
    }
}