using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAClassifieds.Data;
using System.Threading.Tasks;
using TAClassifieds.Model;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Web;

namespace TAClassifieds.BAL
{
   public class AccountBL
    {
        UnitOfWork uw = new UnitOfWork();
        public Boolean UserRegistration(User model)
        {
          //  UnitOfWork uw = new UnitOfWork();
            IEnumerable<User> u = uw.UserRepository.Get(c => c.Email == model.Email);
            if (u.Count() == 0)
            {
                string encryptedpwd = Encryption(model.UPassword);
                var newuser = new User() { Email = model.Email, UPassword = encryptedpwd, UserId = Guid.NewGuid(), CreatedDate = DateTime.UtcNow };
                var insertedUser = uw.UserRepository.Insert(newuser);
                uw.Save();
                AccountActivation(insertedUser);
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean UserVerification(User model)
        {
            UnitOfWork uw = new UnitOfWork();
            string password = Encryption(model.UPassword);
            IEnumerable<User> u = uw.UserRepository.Get(c => c.Email == model.Email && c.UPassword==password);
            if (u.Count() == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean UserProfileStatus(string email)
        {
           
            IEnumerable<User> u = uw.UserRepository.Get(c => c.Email == email);
            var e = u.FirstOrDefault();
            if (e.First_Name != null && e.Last_Name != null && e.Gender != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public User FetchUser(string email)
        {          
            IEnumerable<User> u = uw.UserRepository.Get(c => c.Email == email);
            return u.FirstOrDefault();            
        }
        public void UpdateProfile(User model)
        {           
            User op = FetchUser(model.Email);
            //op.UserId = FetchUserId(model.Email);
           // op.Email = model.Email;
            op.First_Name = model.First_Name;
            op.Last_Name = model.Last_Name;
            op.Address1 = model.Address1;             
            uw.UserRepository.Update(op);
            //uw._context.Users.Attach(op);            
            //uw._context.Entry(op).State = EntityState.Modified;
            uw.Save();
            
        }

        public void Confirmation(Guid userid)
        {            
            //UnitOfWork uw = new UnitOfWork();
            User op = uw.UserRepository.GetByID(userid);
            op.UserId = userid;
            op.IsVerified = true;
            uw.UserRepository.Update(op);
            //uw._context.Users.Attach(op);
           // var entry = uw._context.Entry(op);
           // entry.Property(e => e.IsVerified).IsModified = true;
            uw.Save();
        }

        private void AccountActivation(User model)
        {
            MailMessage mailmessage = new MailMessage();
            mailmessage.IsBodyHtml = true;
            string ActivationUrl = HttpContext.Current.Server.HtmlEncode("http://localhost:57864/Account/Confirmation" + "?id=" + model.UserId);
            mailmessage.Subject = "Confirmation email for account activation- TA Classifieds";
            mailmessage.Body = "Hi," + "!\n" + "Please <a href='" + ActivationUrl + "'>click here</a> the following link to activate your account." + "!\n" + "TA Classifieds.";
            mailmessage.From = new MailAddress("techaspectclassifieds@gmail.com");
            mailmessage.To.Add(model.Email);
            SmtpClient smtp = new SmtpClient();
            smtp.Send(mailmessage);
            
        }
        private string Encryption(string data)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(data);
            byte[] inArray = HashAlgorithm.Create("SHA1").ComputeHash(bytes);
            return Convert.ToBase64String(inArray);
        }


    }
}
