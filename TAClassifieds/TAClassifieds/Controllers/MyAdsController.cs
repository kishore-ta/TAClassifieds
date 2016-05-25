using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TAClassifieds.Data;
using TAClassifieds.Model;
using TAClassifieds.BAL;
using System.Security.Claims;

namespace TAClassifieds.Controllers
{
    public class MyAdsController : Controller
    {
        //
        // GET: /MyAds/
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetMyAds(ClassifiedContactVM Model, int? page)
        {
            try
            {               
                //Get Current UserId
                var identity = (ClaimsIdentity)User.Identity;
                IEnumerable<Claim> claims = identity.Claims;
                var guid = claims.Where(m => m.Type == "guid");
                var UserId = guid.FirstOrDefault().Value;

                //To Get current user Ads
                Model = ClassifiedBAL.GetMyAdsBAL(Model, UserId);

                //Pagination
                var dummyItems = Model.classifiedList;
                var pager = new Pager(dummyItems.Count(), page);
                Model.classifiedList = dummyItems.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize);
                Model.Pager = pager;
            }
            catch (Exception ex)
            {

            }
            return View(Model);
        }

        [HttpGet]
        public ActionResult ClassifiedDetails(Classified Model, int classifiedId)
        {
            try
            {
                Model = ClassifiedBAL.GetClassifiedByIdBAL(Model, classifiedId);
                ViewBag.PostedDate = Convert.ToDateTime(Model.PostedDate).ToString("MMMM-dd-yyyy");
            }
            catch (Exception ex)
            {

            }
            return View(Model);
        }
	}
}