using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TAClassifieds.Data;
using TAClassifieds.Model;
using TAClassifieds.BAL;

namespace TAClassifieds.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetAd(ClassifiedContactVM Model, string categoryvalue, string categoryName, int? page)
        {
            try
            {
                //To Get Category Ads
                Model = ClassifiedBAL.GetAllAdsBAL(Model, categoryvalue, categoryName);

                if (!string.IsNullOrEmpty(categoryvalue))
                {
                    ViewBag.CatagoryName = categoryName;
                }

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

