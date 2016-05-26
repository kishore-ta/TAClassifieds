using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAClassifieds.Data;
using TAClassifieds.Model;
using System.Threading.Tasks;
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
            IEnumerable<User> u = uw.UserRepository.Get(c => c.Email == model.Email);
            var userfromDB = u.FirstOrDefault();
            if (userfromDB != null)
            {

                if (userfromDB.IsVerified == true && userfromDB.IsActive == true)
                {
                    return false;
                }
                else
                    return true;
            }
            else
                return true;

            //var isVerified = u.Where(user => !user.IsVerified.HasValue);

            //return isVerified != null;
            //if (isVerified!=null)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }
        public void Registration(User model, Boolean isSocialLogin)
        {
            //check for user exist
            var user = FetchUser(model.Email);
            if (user != null && !user.IsVerified.HasValue)
            {
                //send new verification link
                var tokenverification1 = new VerifyToken() { TokenId = Guid.NewGuid(), UserId = user.UserId, CreatedDate = DateTime.Now };
                var userVerification1 = uw.VerifyTokenRepository.Insert(tokenverification1);
                uw.Save();
                if (!isSocialLogin)
                {
                    AccountActivation(user, userVerification1);
                }

            }
            else
            {
                string encryptedpwd = Encryption(model.UPassword);
                var newuser = new User() { Email = model.Email, UPassword = encryptedpwd, UserId = Guid.NewGuid(), CreatedDate = DateTime.Now, IsVerified = model.IsVerified != null ? model.IsVerified : null, IsActive = model.IsActive != null ? model.IsActive : null };
                var insertedUser = uw.UserRepository.Insert(newuser);
                uw.Save();
                var tokenverification = new VerifyToken() { TokenId = Guid.NewGuid(), UserId = newuser.UserId, CreatedDate = DateTime.Now };
                var userVerification = uw.VerifyTokenRepository.Insert(tokenverification);
                uw.Save();
                if (!isSocialLogin)
                {
                    AccountActivation(insertedUser, userVerification);
                }
            }

        }
        public User UserVerification(User model)
        {
            string password = Encryption(model.UPassword);
            IEnumerable<User> u = uw.UserRepository.Get(c => c.Email == model.Email && c.UPassword == password);
            User op = null;
            foreach (User user in u)
            {
                if (!user.IsActive.Equals(false) && (user.IsVerified.Equals(true)))
                {
                    op = user;
                    break;
                }
            }
            return op;
        }
        public User FetchUser(string email)
        {
            IEnumerable<User> u = uw.UserRepository.Get(c => c.Email == email);
            return u.FirstOrDefault();
        }
        public User FetchUserInfo(Guid userid)
        {
            return uw.UserRepository.GetByID(userid);
        }
        public void UpdateProfile(User model)
        {
            User op = FetchUserInfo(model.UserId);
            op.First_Name = model.First_Name;
            op.Last_Name = model.Last_Name;
            op.Address1 = model.Address1;
            op.Gender = model.Gender;
            op.DOB = model.DOB;
            op.Address2 = model.Address2;
            op.City = model.City;
            op.State = model.State;
            op.Country = model.Country;
            op.Phone = model.Phone;
            uw.UserRepository.Update(op);
            //uw._context.Users.Attach(op);            
            //uw._context.Entry(op).State = EntityState.Modified;
            uw.Save();
        }
        public Boolean Confirmation(Guid tokenid)
        {

            VerifyToken vp = uw.VerifyTokenRepository.GetByID(tokenid);
            //if (vp.CreatedDate.AddHours(24)>DateTime.Now)
            if (vp.CreatedDate.AddMinutes(1) > DateTime.Now && vp.IsUsed != true)
            {
                vp.TokenId = tokenid;
                vp.IsUsed = true;
                uw.VerifyTokenRepository.Update(vp);
                uw.Save();
                Guid userid = vp.UserId;
                User op = uw.UserRepository.GetByID(userid);
                op.UserId = userid;
                op.IsVerified = true;
                op.IsActive = true;
                uw.UserRepository.Update(op);
                //uw._context.Users.Attach(op);
                // var entry = uw._context.Entry(op);
                // entry.Property(e => e.IsVerified).IsModified = true;
                uw.Save();
                return true;
            }
            else
            {
                vp.IsExpired = true;
                uw.VerifyTokenRepository.Update(vp);
                uw.Save();
                Guid userid = vp.UserId;
                User op = uw.UserRepository.GetByID(userid);
                //op.IsActive = false;
                uw.UserRepository.Update(op);
                uw.Save();
                return false;
            }
        }
        private void AccountActivation(User usermodel, VerifyToken verificationmodel)
        {
            MailMessage mailmessage = new MailMessage();
            mailmessage.IsBodyHtml = true;
            string ActivationUrl = HttpContext.Current.Server.HtmlEncode("http://localhost:57864/Account/Confirmation" + "?id=" + verificationmodel.TokenId);
            mailmessage.Subject = "Confirmation email for account activation- TA Classifieds";
            mailmessage.Body = "Hi," + "!\n" + "Please <a href='" + ActivationUrl + "'>click here</a> the following link to activate your account." + "!\n" + "TA Classifieds.";
            mailmessage.From = new MailAddress("techaspectclassifieds@gmail.com");
            mailmessage.To.Add(usermodel.Email);
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
