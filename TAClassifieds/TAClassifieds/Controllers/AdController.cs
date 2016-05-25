using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TAClassifieds.Model;
using TAClassifieds.BAL;
using System.Security.Claims;

namespace TAClassifieds.Controllers
{
    [Authorize]
    public class AdController : Controller
    {
        //
        // GET: /PostAd/
        public ActionResult Index()
        {
            return View("PostAd");
        }

        [HttpGet]
        public ActionResult PostAd(ClassifiedContactVM Model)
        {
            try
            {               
                //To Get Categories of Ads
                Model = ClassifiedBAL.GetCategoriesBAL(Model);

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
            int excFlag = 0;

            //Get Current UserId
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            var guid = claims.Where(m => m.Type == "guid");
            var UserId = guid.FirstOrDefault().Value;

            //To Post the Ad information which is entered by user.
            excFlag = ClassifiedBAL.PostAdBAL(Model, categoryvalue, file,UserId);

            if (excFlag > 0)
            {
                ViewBag.successMessage = "Your Ad created successfully.";
            }
            return RedirectToAction("PostAd");
        }
    }
}
