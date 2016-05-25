using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAClassifieds.Model;
using System.Web;
using System.Data.SqlClient;

namespace TAClassifieds.BAL
{
    public class ClassifiedBAL
    {
        //Get Ads method for PostAd Controller, 
        public static ClassifiedContactVM GetCategoriesBAL(ClassifiedContactVM Model)
        {
            UnitOfWork uw = new UnitOfWork();
            Model.categoriesList = uw.CategoryRepository.Get().ToList();
            return Model;
        }

        //Post Ads method for PostAd Controller,
        public static int PostAdBAL(ClassifiedContactVM Model, string categoryvalue, HttpPostedFileBase file,string userid)
        {
            int excFlag = 0;
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
                 path = HttpContext.Current.Server.MapPath(strPath);
               // path = Path.GetDirectoryName(strPath);
                file.SaveAs(path);
                objClassified.ClassifiedImage = strPath;
            }

            objClassified.ClassifiedPrice = Convert.ToInt32(Model.ClassifiedPrice);
            objClassified.Summary = Model.Description;
            objClassified.PostedDate = DateTime.Now;

            objClassified.CreatedBy = Guid.Parse(userid);

            ClassifiedContact objContact = Model.classifiedsContacts;

            Classified cls = uw.ClassifiedRepository.Insert(objClassified);
            objContact.ClassifiedId = cls.ClassifiedId;

            uw.ClassifiedContactRepository.Insert(objContact);
            excFlag = uw.Save();

            return excFlag;
        }

        public static ClassifiedContactVM GetAllAdsBAL(ClassifiedContactVM Model, string categoryvalue, string categoryName)
        {

            UnitOfWork uw = new UnitOfWork();
            Model.categoriesList = uw.CategoryRepository.Get().ToList();
            if (!string.IsNullOrEmpty(categoryvalue))
            {
                Model.classifiedList = uw.ClassifiedRepository.GetWithRawSql("select * from TAC_Classified c Join TAC_ClassifiedContact cc ON c.ClassifiedId=cc.ClassifiedId where CategoryId=@categoryId order by PostedDate DESC", new SqlParameter("@categoryId", Convert.ToInt32(categoryvalue)));
                //ViewBag.CatagoryName = categoryName;
            }
            else
            {
                Model.classifiedList = uw.ClassifiedRepository.GetWithRawSql("select * from TAC_Classified c Join TAC_ClassifiedContact cc ON c.ClassifiedId=cc.ClassifiedId order by PostedDate DESC");
            }

            return Model;
        }

        public static Classified GetClassifiedByIdBAL(Classified Model, int classifiedId)
        {
            UnitOfWork uw = new UnitOfWork();
            Model = uw.ClassifiedRepository.GetByID(classifiedId);
            return Model;
        }

        public static ClassifiedContactVM GetMyAdsBAL(ClassifiedContactVM Model,string UserId)
        {
            UnitOfWork uw = new UnitOfWork();
            if (!string.IsNullOrEmpty(UserId))
            {
                Model.classifiedList = uw.ClassifiedRepository.GetWithRawSql("select * from TAC_Classified c Join TAC_ClassifiedContact cc ON c.ClassifiedId=cc.ClassifiedId where CreatedBy=@createdBy order by PostedDate DESC", new SqlParameter("@createdBy", Guid.Parse(UserId)));
                //ViewBag.CatagoryName = categoryName;
            }           

            return Model;
        }
    }
}
