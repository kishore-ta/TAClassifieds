using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

        [HttpGet]
        public ActionResult GetAd(ClassifiedContactVM Model, string categoryvalue, string categoryName)
        {
            try
            {
                ViewBag.CatagoryName = string.Empty;
                UnitOfWork uw = new UnitOfWork();
                Model.categoriesList = uw.CategoryRepository.Get().ToList();

                if (!string.IsNullOrEmpty(categoryvalue))
                {
                    Model.classifiedList = uw.ClassifiedRepository.GetWithRawSql("select * from TAC_Classified c Join TAC_ClassifiedContact cc ON c.ClassifiedId=cc.ClassifiedId where CategoryId=@categoryId order by PostedDate DESC", new SqlParameter("@categoryId", Convert.ToInt32(categoryvalue)));
                    ViewBag.CatagoryName = categoryName;
                }
                else
                {
                    Model.classifiedList = uw.ClassifiedRepository.GetWithRawSql("select * from TAC_Classified c Join TAC_ClassifiedContact cc ON c.ClassifiedId=cc.ClassifiedId order by PostedDate DESC");
                }

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
                UnitOfWork uw = new UnitOfWork();
                Model = uw.ClassifiedRepository.GetByID(classifiedId);
                ViewBag.PostedDate = Convert.ToDateTime(Model.PostedDate).ToString("MMMM-dd-yyyy");
            }
            catch(Exception ex)
            {

            }
            return View(Model);
        }


    }
}

